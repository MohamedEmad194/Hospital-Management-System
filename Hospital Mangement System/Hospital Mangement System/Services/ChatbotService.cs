using Hospital_Management_System.DTOs;
using Hospital_Management_System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hospital_Management_System.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly HospitalDbContext _context;
        private readonly ILogger<ChatbotService> _logger;
        private readonly IOpenAIService? _openAIService = null;
        private readonly IConfiguration _configuration;
        private readonly bool _useOpenAI;
        private readonly string _defaultLanguage;

        // Symptom to specialization mapping (Arabic and English)
        private readonly Dictionary<string, string[]> _symptomToSpecialization = new()
        {
            { "heart", new[] { "Cardiology", "أمراض القلب", "قلب", "ضغط", "ضغط الدم", "chest pain", "ألم صدر", "صدر" } },
            { "head", new[] { "Neurology", "أمراض عصبية", "رأس", "صداع", "headache", "مخ", "أعصاب" } },
            { "stomach", new[] { "Gastroenterology", "أمراض الجهاز الهضمي", "بطن", "معدة", "هضم", "digestion", "إسهال", "إمساك" } },
            { "bone", new[] { "Orthopedics", "جراحة العظام", "عظام", "مفاصل", "joints", "ركبة", "ظهر", "back" } },
            { "eye", new[] { "Ophthalmology", "طب العيون", "عيون", "عين", "نظر", "vision" } },
            { "ear", new[] { "ENT", "أنف وأذن وحنجرة", "أذن", "أنف", "حنجرة", "nose", "throat" } },
            { "skin", new[] { "Dermatology", "طب الجلدية", "جلد", "حساسية", "allergy", "طفح" } },
            { "child", new[] { "Pediatrics", "طب الأطفال", "أطفال", "طفل", "رضيع", "baby" } },
            { "woman", new[] { "Gynecology", "طب النساء", "نساء", "حمل", "pregnancy", "ولادة" } },
            { "mental", new[] { "Psychiatry", "طب نفسي", "نفسي", "اكتئاب", "depression", "قلق", "anxiety" } },
            { "cancer", new[] { "Oncology", "أورام", "سرطان", "ورم" } },
            { "kidney", new[] { "Nephrology", "أمراض الكلى", "كلى", "كلية", "بول", "urine" } }
        };

        public ChatbotService(
            HospitalDbContext context, 
            ILogger<ChatbotService> logger,
            IConfiguration configuration,
            IOpenAIService? openAIService = null)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _openAIService = openAIService;
            _useOpenAI = configuration.GetValue<bool>("ChatbotSettings:UseOpenAI", false) && 
                        !string.IsNullOrEmpty(configuration["ChatbotSettings:OpenAIApiKey"]);
            _defaultLanguage = configuration["ChatbotSettings:DefaultLanguage"] ?? "ar";
        }

        public async Task<ChatbotResponseDto> ProcessMessageAsync(ChatbotMessageDto message)
        {
            var userMessage = message.Message.Trim();
            var userMessageLower = userMessage.ToLower();
            var response = new ChatbotResponseDto();
            
            // Always prefer Arabic as default, but detect if user is using English
            var isArabic = _defaultLanguage == "ar" || IsArabicMessage(userMessage);
            
            // Try OpenAI first if enabled
            if (_useOpenAI && _openAIService != null)
            {
                try
                {
                    var context = await BuildContextForAI();
                    var aiResponse = await _openAIService.GetChatResponseAsync(userMessage, context);
                    
                    if (!string.IsNullOrEmpty(aiResponse))
                    {
                        return new ChatbotResponseDto
                        {
                            Response = aiResponse,
                            Suggestions = GetDefaultSuggestions(isArabic)
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "OpenAI service failed, falling back to rule-based");
                }
            }

            // Detect symptoms and suggest doctors
            var detectedSymptom = DetectSymptom(userMessageLower);
            if (detectedSymptom != null)
            {
                response = await SuggestDoctorForSymptom(detectedSymptom, isArabic);
                return response;
            }

            // Appointment booking with smart suggestions
            if (ContainsKeywords(userMessageLower, new[] { "appointment", "موعد", "حجز", "book", "schedule", "احجز", "حجز موعد", "موعد مع طبيب" }))
            {
                response = await GetAppointmentInfo(isArabic);
                return response;
            }

            // Doctor search
            if (ContainsKeywords(userMessageLower, new[] { "doctor", "طبيب", "specialist", "specialization", "تخصص", "دكتور", "أريد طبيب", "ابحث عن طبيب" }))
            {
                response = await GetDoctorInfo(userMessageLower, isArabic);
                return response;
            }

            // Department info
            if (ContainsKeywords(userMessageLower, new[] { "department", "قسم", "departments", "أقسام", "أي قسم" }))
            {
                response = await GetDepartmentInfo(isArabic);
                return response;
            }

            // Bill/Payment
            if (ContainsKeywords(userMessageLower, new[] { "bill", "فاتورة", "payment", "دفع", "pay", "سداد", "فاتورة", "مبلغ" }))
            {
                response = await GetBillInfo(isArabic);
                return response;
            }

            // Medicine
            if (ContainsKeywords(userMessageLower, new[] { "medicine", "دواء", "prescription", "وصفة", "drug", "أدوية", "صيدلية" }))
            {
                response = await GetMedicineInfo(isArabic);
                return response;
            }

            // Emergency
            if (ContainsKeywords(userMessageLower, new[] { "emergency", "طوارئ", "urgent", "عاجل", "طارئ", "حالة طوارئ" }))
            {
                response = GetEmergencyInfo(isArabic);
                return response;
            }

            // Working hours
            if (ContainsKeywords(userMessageLower, new[] { "hours", "ساعات", "working", "عمل", "open", "مفتوح", "متى", "وقت" }))
            {
                response = GetWorkingHoursInfo(isArabic);
                return response;
            }

            // Location
            if (ContainsKeywords(userMessageLower, new[] { "location", "موقع", "address", "عنوان", "where", "أين", "عنوان المستشفى" }))
            {
                response = GetLocationInfo(isArabic);
                return response;
            }

            // Insurance
            if (ContainsKeywords(userMessageLower, new[] { "insurance", "تأمين", "coverage", "تغطية", "تأمين صحي" }))
            {
                response = GetInsuranceInfo(isArabic);
                return response;
            }

            // Treatment suggestions
            if (ContainsKeywords(userMessageLower, new[] { "treatment", "علاج", "cure", "كيف أعالج", "ماذا أفعل", "علاج ل", "علاج من" }))
            {
                response = await GetTreatmentSuggestion(userMessageLower, isArabic);
                return response;
            }

            // Greeting
            if (ContainsKeywords(userMessageLower, new[] { "hello", "مرحبا", "hi", "السلام", "help", "مساعدة", "help me", "أهلا", "السلام عليكم" }))
            {
                response = GetGreeting(isArabic);
                return response;
            }

            // Default response
            response = GetDefaultResponse(isArabic);
            return response;
        }

        private bool IsArabicMessage(string message)
        {
            // Check if message contains Arabic characters
            return message.Any(c => c >= 0x0600 && c <= 0x06FF);
        }

        private string? DetectSymptom(string message)
        {
            foreach (var kvp in _symptomToSpecialization)
            {
                if (kvp.Value.Any(keyword => message.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        private async Task<ChatbotResponseDto> SuggestDoctorForSymptom(string symptom, bool isArabic)
        {
            try
            {
                var specializations = _symptomToSpecialization[symptom];
                var specialization = specializations[0]; // Get the English specialization name

                // Load active doctors into memory first to safely use string.Contains with StringComparison
                var activeDoctors = await _context.Doctors
                    .Include(d => d.Department)
                    .Where(d => d.IsActive)
                    .ToListAsync();

                var doctors = activeDoctors
                    .Where(d => !string.IsNullOrEmpty(d.Specialization) &&
                                (d.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase) ||
                                 specializations.Any(s => d.Specialization.Contains(s, StringComparison.OrdinalIgnoreCase))))
                    .Take(3)
                    .ToList();

                if (isArabic)
                {
                    if (doctors.Any())
                    {
                        var doctorList = string.Join("\n", doctors.Select((d, i) => 
                            $"{i + 1}. د. {d.FirstName} {d.LastName} - {d.Specialization}\n   القسم: {d.Department?.Name ?? "غير محدد"}\n   الخبرة: {d.YearsOfExperience} سنة\n   رسوم الاستشارة: {d.ConsultationFee} جنيه"));

                        var response = new ChatbotResponseDto
                        {
                            Response = $"بناءً على وصفك، أنصحك بالتواصل مع أحد الأطباء التاليين:\n\n{doctorList}\n\n" +
                                      "يمكنك حجز موعد مباشرة من قسم المواعيد. هل تريد المساعدة في حجز موعد؟",
                            Suggestions = new List<string> 
                            { 
                                "احجز موعد مع هذا الطبيب", 
                                "أعطني المزيد من المعلومات", 
                                "ما هي الأوقات المتاحة؟",
                                "Book appointment with this doctor",
                                "More information",
                                "Available times"
                            }
                        };
                        return response;
                    }
                    else
                    {
                        return new ChatbotResponseDto
                        {
                            Response = "عذراً، لم أجد أطباء متخصصين في هذا المجال حالياً. يرجى التواصل مع قسم الاستقبال للحصول على المساعدة.",
                            Suggestions = new List<string> { "أقسام المستشفى", "مواعيد متاحة", "اتصل بالاستقبال" }
                        };
                    }
                }
                else
                {
                    if (doctors.Any())
                    {
                        var doctorList = string.Join("\n", doctors.Select((d, i) => 
                            $"{i + 1}. Dr. {d.FirstName} {d.LastName} - {d.Specialization}\n   Department: {d.Department?.Name ?? "N/A"}\n   Experience: {d.YearsOfExperience} years\n   Consultation Fee: {d.ConsultationFee} EGP"));

                        var response = new ChatbotResponseDto
                        {
                            Response = $"Based on your description, I recommend consulting with one of these doctors:\n\n{doctorList}\n\n" +
                                      "You can book an appointment directly from the Appointments section. Would you like help booking an appointment?",
                            Suggestions = new List<string> 
                            { 
                                "Book appointment with this doctor", 
                                "More information", 
                                "What are available times?",
                                "احجز موعد مع هذا الطبيب",
                                "مزيد من المعلومات"
                            }
                        };
                        return response;
                    }
                    else
                    {
                        return new ChatbotResponseDto
                        {
                            Response = "Sorry, I couldn't find specialists in this field currently. Please contact our reception for assistance.",
                            Suggestions = new List<string> { "Hospital departments", "Available appointments", "Contact reception" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suggesting doctor for symptom");
                return new ChatbotResponseDto
                {
                    Response = isArabic 
                        ? "حدث خطأ في البحث. يرجى المحاولة مرة أخرى أو التواصل مع قسم الاستقبال."
                        : "An error occurred. Please try again or contact reception.",
                    Suggestions = new List<string> { isArabic ? "اتصل بالاستقبال" : "Contact reception" }
                };
            }
        }

        private async Task<ChatbotResponseDto> GetAppointmentInfo(bool isArabic)
        {
            try
            {
                var availableDoctors = await _context.Doctors
                    .Where(d => d.IsActive && d.IsAvailable)
                    .CountAsync();

                var upcomingAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate >= DateTime.Today && a.Status == "Scheduled")
                    .CountAsync();

                // Get available time slots (next 7 days)
                var today = DateTime.Today;
                var availableSlots = new List<string>();
                
                for (int i = 0; i < 7; i++)
                {
                    var date = today.AddDays(i);
                    var dayName = isArabic 
                        ? date.ToString("dddd", new System.Globalization.CultureInfo("ar-EG"))
                        : date.ToString("dddd");
                    
                    if (i == 0)
                        availableSlots.Add(isArabic ? $"اليوم ({dayName})" : $"Today ({dayName})");
                    else if (i == 1)
                        availableSlots.Add(isArabic ? $"غداً ({dayName})" : $"Tomorrow ({dayName})");
                    else
                        availableSlots.Add($"{dayName} - {date:dd/MM}");
                }

                if (isArabic)
                {
                    return new ChatbotResponseDto
                    {
                        Response = $"لدينا {availableDoctors} طبيب متاح حالياً.\n\n" +
                                  "لحجز موعد:\n" +
                                  "1. اذهب إلى قسم المواعيد\n" +
                                  "2. اختر الطبيب المناسب\n" +
                                  "3. اختر التاريخ والوقت\n" +
                                  "4. أكد الحجز\n\n" +
                                  $"المواعيد المتاحة للأيام القادمة:\n{string.Join("\n", availableSlots)}\n\n" +
                                  "يمكنك أيضاً الاتصال بخط المواعيد للمساعدة.",
                        Suggestions = new List<string> 
                        { 
                            "احجز موعد الآن", 
                            "أعطني قائمة الأطباء", 
                            "ما هي الأوقات المتاحة؟",
                            "Book appointment now",
                            "List of doctors",
                            "Available times"
                        }
                    };
                }
                else
                {
                    return new ChatbotResponseDto
                    {
                        Response = $"We currently have {availableDoctors} available doctors.\n\n" +
                                  "To book an appointment:\n" +
                                  "1. Go to the Appointments section\n" +
                                  "2. Select your preferred doctor\n" +
                                  "3. Choose date and time\n" +
                                  "4. Confirm your booking\n\n" +
                                  $"Available slots for the next days:\n{string.Join("\n", availableSlots)}\n\n" +
                                  "You can also call our appointment line for assistance.",
                        Suggestions = new List<string> 
                        { 
                            "Book appointment now", 
                            "List of doctors", 
                            "What are available times?",
                            "احجز موعد الآن",
                            "قائمة الأطباء"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment info");
                return new ChatbotResponseDto
                {
                    Response = isArabic 
                        ? "يمكنك حجز المواعيد من خلال نظامنا. يرجى زيارة قسم المواعيد أو الاتصال بالاستقبال."
                        : "You can book appointments through our system. Please visit the Appointments section or contact reception.",
                    Suggestions = new List<string> { isArabic ? "اتصل بالاستقبال" : "Contact reception" }
                };
            }
        }

        private async Task<ChatbotResponseDto> GetDoctorInfo(string message, bool isArabic)
        {
            try
            {
                // Try to extract specialization from message
                var doctors = await _context.Doctors
                    .Include(d => d.Department)
                    .Where(d => d.IsActive)
                    .ToListAsync();

                var departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => d.Name)
                    .ToListAsync();

                // Check if user is asking for specific specialization
                var allSpecializations = doctors.Select(d => d.Specialization).Distinct().ToList();
                var matchedSpecialization = allSpecializations.FirstOrDefault(s => 
                    message.Contains(s, StringComparison.OrdinalIgnoreCase));

                List<Models.Doctor> filteredDoctors;
                if (matchedSpecialization != null)
                {
                    filteredDoctors = doctors.Where(d => 
                        d.Specialization.Contains(matchedSpecialization, StringComparison.OrdinalIgnoreCase))
                        .Take(5).ToList();
                }
                else
                {
                    filteredDoctors = doctors.Take(5).ToList();
                }

                if (isArabic)
                {
                    var doctorList = string.Join("\n\n", filteredDoctors.Select((d, i) => 
                        $"{i + 1}. د. {d.FirstName} {d.LastName}\n" +
                        $"   التخصص: {d.Specialization}\n" +
                        $"   القسم: {d.Department?.Name ?? "غير محدد"}\n" +
                        $"   الخبرة: {d.YearsOfExperience} سنة\n" +
                        $"   رسوم الاستشارة: {d.ConsultationFee} جنيه"));

                    return new ChatbotResponseDto
                    {
                        Response = $"لدينا أطباء متمرسون في مختلف التخصصات.\n\n" +
                                  (departments.Any() ? $"أقسامنا تشمل: {string.Join("، ", departments)}.\n\n" : "") +
                                  $"بعض الأطباء المتاحين:\n\n{doctorList}\n\n" +
                                  "يمكنك عرض جميع الأطباء في قسم الأطباء وحجز موعد مع الطبيب المفضل لديك.",
                        Suggestions = new List<string> 
                        { 
                            "احجز موعد", 
                            "أعطني المزيد من الأطباء", 
                            "ما هي التخصصات المتاحة؟",
                            "Book appointment",
                            "More doctors",
                            "Available specializations"
                        }
                    };
                }
                else
                {
                    var doctorList = string.Join("\n\n", filteredDoctors.Select((d, i) => 
                        $"{i + 1}. Dr. {d.FirstName} {d.LastName}\n" +
                        $"   Specialization: {d.Specialization}\n" +
                        $"   Department: {d.Department?.Name ?? "N/A"}\n" +
                        $"   Experience: {d.YearsOfExperience} years\n" +
                        $"   Consultation Fee: {d.ConsultationFee} EGP"));

                    return new ChatbotResponseDto
                    {
                        Response = $"We have experienced doctors in various specialties.\n\n" +
                                  (departments.Any() ? $"Our departments include: {string.Join(", ", departments)}.\n\n" : "") +
                                  $"Some available doctors:\n\n{doctorList}\n\n" +
                                  "You can view all available doctors in the Doctors section and book an appointment with your preferred specialist.",
                        Suggestions = new List<string> 
                        { 
                            "Book appointment", 
                            "More doctors", 
                            "What specializations are available?",
                            "احجز موعد",
                            "المزيد من الأطباء"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor info");
                return new ChatbotResponseDto
                {
                    Response = isArabic 
                        ? "لدينا أطباء متمرسون في مختلف التخصصات. يرجى زيارة قسم الأطباء لرؤية جميع الأطباء المتاحين."
                        : "We have experienced doctors in various specialties. Please visit the Doctors section to see all available doctors.",
                    Suggestions = new List<string> { isArabic ? "قائمة الأطباء" : "List of doctors" }
                };
            }
        }

        private async Task<ChatbotResponseDto> GetDepartmentInfo(bool isArabic)
        {
            try
            {
                var departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => new { d.Name, d.Description })
                    .ToListAsync();

                if (departments.Any())
                {
                    var deptList = string.Join("\n\n", departments.Select((d, i) => 
                        $"{i + 1}. {d.Name}" + 
                        (!string.IsNullOrEmpty(d.Description) ? $"\n   {d.Description}" : "")));

                    return new ChatbotResponseDto
                    {
                        Response = isArabic
                            ? $"أقسام المستشفى:\n\n{deptList}\n\nكل قسم مجهز بأخصائيين متمرسين. يمكنك العثور على مزيد من التفاصيل في قسم الأقسام."
                            : $"Hospital Departments:\n\n{deptList}\n\nEach department is staffed with experienced specialists. You can find more details in the Departments section.",
                        Suggestions = new List<string> 
                        { 
                            isArabic ? "احجز موعد" : "Book appointment",
                            isArabic ? "قائمة الأطباء" : "List of doctors",
                            isArabic ? "خدمات الأقسام" : "Department services"
                        }
                    };
                }

                return new ChatbotResponseDto
                {
                    Response = isArabic
                        ? "لدينا أقسام متخصصة متعددة لخدمة احتياجاتك الصحية. يرجى زيارة قسم الأقسام لمزيد من المعلومات."
                        : "We have multiple specialized departments to serve your healthcare needs. Please visit the Departments section for detailed information.",
                    Suggestions = new List<string> { isArabic ? "الأقسام المتاحة" : "Available departments" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department info");
                return new ChatbotResponseDto
                {
                    Response = isArabic
                        ? "لدينا أقسام متخصصة متعددة. يرجى زيارة قسم الأقسام لمزيد من المعلومات."
                        : "We have multiple specialized departments. Please visit the Departments section for detailed information.",
                    Suggestions = new List<string> { isArabic ? "معلومات الأقسام" : "Department information" }
                };
            }
        }

        private async Task<ChatbotResponseDto> GetBillInfo(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "يمكنك عرض فواتيرك في قسم الفواتير. لاستفسارات الدفع، يرجى التواصل مع قسم الفواتير في المستشفى أو الاتصال بخط الدعم.\n\n" +
                      "طرق الدفع المتاحة:\n" +
                      "• نقداً في قسم الفواتير\n" +
                      "• بطاقة ائتمان/خصم\n" +
                      "• تحويل بنكي\n" +
                      "• التأمين الصحي (إذا كان متاحاً)"
                    : "You can view your bills in the Bills section. For payment inquiries, please contact our billing department at the hospital or call our support line.\n\n" +
                      "Available payment methods:\n" +
                      "• Cash at billing department\n" +
                      "• Credit/Debit card\n" +
                      "• Bank transfer\n" +
                      "• Health insurance (if applicable)",
                Suggestions = new List<string> 
                { 
                    isArabic ? "كيف أدفع الفواتير؟" : "How to pay bills?",
                    isArabic ? "حالة الفاتورة" : "Bill status",
                    isArabic ? "استفسار عن فاتورة" : "Bill inquiry"
                }
            };
        }

        private async Task<ChatbotResponseDto> GetMedicineInfo(bool isArabic)
        {
            try
            {
                var medicineCount = await _context.Medicines.CountAsync(m => m.IsActive);
                var lowStockCount = await _context.Medicines.CountAsync(m => m.IsActive && m.StockQuantity <= m.MinimumStockLevel);

                return new ChatbotResponseDto
                {
                    Response = isArabic
                        ? $"لدينا {medicineCount} دواء متاح في صيدليتنا.\n" +
                          (lowStockCount > 0 ? $"ملاحظة: {lowStockCount} دواء حالياً قليل المخزون.\n" : "") +
                          "يمكنك عرض جميع الأدوية المتاحة في قسم الأدوية. لتجديد الوصفات، يرجى التواصل مع طبيبك أو زيارة صيدليتنا."
                        : $"We have {medicineCount} medicines available in our pharmacy.\n" +
                          (lowStockCount > 0 ? $"Note: {lowStockCount} medicines are currently low in stock.\n" : "") +
                          "You can view all available medicines in the Medicines section. For prescription refills, please contact your doctor or visit our pharmacy.",
                    Suggestions = new List<string> 
                    { 
                        isArabic ? "توفر الدواء" : "Medicine availability",
                        isArabic ? "تجديد الوصفة" : "Prescription refill",
                        isArabic ? "قائمة الأدوية" : "List of medicines"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medicine info");
                return new ChatbotResponseDto
                {
                    Response = isArabic
                        ? "لدينا صيدلية شاملة مع أدوية متنوعة. يمكنك عرض الأدوية المتاحة في قسم الأدوية."
                        : "We have a comprehensive pharmacy with various medicines. You can view available medicines in the Medicines section.",
                    Suggestions = new List<string> { isArabic ? "الأدوية المتاحة" : "Available medicines" }
                };
            }
        }

        private ChatbotResponseDto GetEmergencyInfo(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "للحالات الطبية الطارئة، يرجى الاتصال برقم 123 فوراً أو زيارة قسم الطوارئ. خدمات الطوارئ متاحة على مدار الساعة.\n\n" +
                      "أرقام الطوارئ:\n" +
                      "• الطوارئ العامة: 123\n" +
                      "• قسم الطوارئ: متاح 24/7\n" +
                      "• الإسعاف: 123"
                    : "For medical emergencies, please call 123 immediately or visit our Emergency Department. Our emergency services are available 24/7.\n\n" +
                      "Emergency Numbers:\n" +
                      "• General Emergency: 123\n" +
                      "• Emergency Department: Available 24/7\n" +
                      "• Ambulance: 123",
                Suggestions = new List<string> 
                { 
                    isArabic ? "جهة اتصال الطوارئ" : "Emergency contact",
                    isArabic ? "خدمات الطوارئ" : "Emergency services",
                    isArabic ? "موقع قسم الطوارئ" : "Emergency department location"
                }
            };
        }

        private ChatbotResponseDto GetWorkingHoursInfo(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "مستشفانا مفتوح على مدار الساعة.\n\n" +
                      "ساعات العمل:\n" +
                      "• الاستشارات العادية: من 8:00 صباحاً إلى 8:00 مساءً\n" +
                      "• خدمات الطوارئ: متاحة على مدار الساعة\n" +
                      "• الصيدلية: من 8:00 صباحاً إلى 10:00 مساءً\n" +
                      "• قسم الفواتير: من 9:00 صباحاً إلى 5:00 مساءً"
                    : "Our hospital is open 24/7.\n\n" +
                      "Working Hours:\n" +
                      "• Regular consultations: 8:00 AM to 8:00 PM\n" +
                      "• Emergency services: Available 24/7\n" +
                      "• Pharmacy: 8:00 AM to 10:00 PM\n" +
                      "• Billing department: 9:00 AM to 5:00 PM",
                Suggestions = new List<string> 
                { 
                    isArabic ? "ساعات الزيارة" : "Visiting hours",
                    isArabic ? "توفر الأطباء" : "Doctor availability",
                    isArabic ? "احجز موعد" : "Book appointment"
                }
            };
        }

        private ChatbotResponseDto GetLocationInfo(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "مستشفانا يقع في المنطقة الطبية الرئيسية.\n\n" +
                      "للحصول على اتجاهات محددة، يرجى:\n" +
                      "• الاتصال بالاستقبال عند المدخل الرئيسي\n" +
                      "• زيارة موقعنا الإلكتروني للحصول على خريطة مفصلة\n" +
                      "• استخدام تطبيق الخرائط للوصول إلينا"
                    : "Our hospital is located at the main medical district.\n\n" +
                      "For specific directions, please:\n" +
                      "• Contact our reception at the main entrance\n" +
                      "• Visit our website for a detailed map\n" +
                      "• Use map applications to reach us",
                Suggestions = new List<string> 
                { 
                    isArabic ? "عنوان المستشفى" : "Hospital address",
                    isArabic ? "الاتجاهات" : "Directions",
                    isArabic ? "كيف أصل للمستشفى؟" : "How to reach hospital?"
                }
            };
        }

        private ChatbotResponseDto GetInsuranceInfo(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "نقبل معظم خطط التأمين الرئيسية.\n\n" +
                      "يرجى إحضار بطاقة التأمين عند الزيارة.\n\n" +
                      "للاستفسارات حول التغطية، يرجى التواصل مع قسم التأمين.\n\n" +
                      "شركات التأمين المقبولة:\n" +
                      "• التأمين الصحي الحكومي\n" +
                      "• شركات التأمين الخاصة\n" +
                      "• التأمين الدولي"
                    : "We accept most major insurance plans.\n\n" +
                      "Please bring your insurance card when visiting.\n\n" +
                      "For specific coverage questions, contact our insurance department.\n\n" +
                      "Accepted Insurance:\n" +
                      "• Government health insurance\n" +
                      "• Private insurance companies\n" +
                      "• International insurance",
                Suggestions = new List<string> 
                { 
                    isArabic ? "التأمين المقبول" : "Insurance accepted",
                    isArabic ? "تفاصيل التغطية" : "Coverage details",
                    isArabic ? "استفسار عن التأمين" : "Insurance inquiry"
                }
            };
        }

        private async Task<ChatbotResponseDto> GetTreatmentSuggestion(string message, bool isArabic)
        {
            var detectedSymptom = DetectSymptom(message);
            
            if (detectedSymptom != null)
            {
                var treatmentAdvice = GetTreatmentAdvice(detectedSymptom, isArabic);
                var doctorSuggestion = await SuggestDoctorForSymptom(detectedSymptom, isArabic);
                
                return new ChatbotResponseDto
                {
                    Response = treatmentAdvice + "\n\n" + doctorSuggestion.Response,
                    Suggestions = doctorSuggestion.Suggestions
                };
            }

            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "للحصول على اقتراحات علاجية مناسبة، يرجى وصف الأعراض التي تعاني منها. يمكنني بعد ذلك توجيهك للطبيب المناسب."
                    : "To get appropriate treatment suggestions, please describe the symptoms you're experiencing. I can then direct you to the appropriate doctor.",
                Suggestions = new List<string> 
                { 
                    isArabic ? "أعاني من ألم" : "I have pain",
                    isArabic ? "أعراضي هي" : "My symptoms are",
                    isArabic ? "ابحث عن طبيب" : "Find a doctor"
                }
            };
        }

        private string GetTreatmentAdvice(string symptom, bool isArabic)
        {
            var advice = new Dictionary<string, (string Arabic, string English)>
            {
                { "heart", ("للأعراض القلبية، يرجى:\n• الراحة التامة\n• تجنب المجهود\n• مراجعة طبيب القلب في أقرب وقت\n• في حالة الطوارئ، اتصل بـ 123 فوراً", "For heart symptoms, please:\n• Rest completely\n• Avoid exertion\n• See a cardiologist as soon as possible\n• In emergency, call 123 immediately") },
                { "head", ("للصداع:\n• الراحة في مكان هادئ\n• شرب الماء\n• تجنب الضوء الساطع\n• إذا استمر، راجع طبيب الأعصاب", "For headaches:\n• Rest in a quiet place\n• Drink water\n• Avoid bright light\n• If persistent, see a neurologist") },
                { "stomach", ("لمشاكل المعدة:\n• تجنب الأطعمة الحارة\n• شرب الماء بكثرة\n• الراحة\n• راجع طبيب الجهاز الهضمي", "For stomach issues:\n• Avoid spicy foods\n• Drink plenty of water\n• Rest\n• See a gastroenterologist") },
                { "bone", ("لآلام العظام:\n• الراحة\n• تجنب الحركة المفرطة\n• كمادات باردة/دافئة\n• راجع طبيب العظام", "For bone pain:\n• Rest\n• Avoid excessive movement\n• Cold/warm compresses\n• See an orthopedist") }
            };

            if (advice.ContainsKey(symptom))
            {
                return isArabic ? advice[symptom].Arabic : advice[symptom].English;
            }

            return isArabic
                ? "يُنصح بمراجعة الطبيب المختص لتشخيص دقيق وعلاج مناسب. يمكنني مساعدتك في العثور على الطبيب المناسب."
                : "It's recommended to see a specialist doctor for accurate diagnosis and appropriate treatment. I can help you find the right doctor.";
        }

        private ChatbotResponseDto GetGreeting(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "مرحباً بك في مستشفى الحياة 👋\nأنا المساعد الذكي لمستشفى الحياة، ويمكنني مساعدتك في:\n\n" +
                      "• حجز المواعيد والبحث عن الأطباء المناسبين\n" +
                      "• معلومات الأقسام والتخصصات\n" +
                      "• استفسارات الفواتير والدفع\n" +
                      "• معلومات الأدوية والصيدلية\n" +
                      "• خدمات الطوارئ\n" +
                      "• ساعات العمل وموقع المستشفى\n" +
                      "• معلومات التأمين\n" +
                      "• اقتراحات العلاج بناءً على الأعراض\n\n" +
                      "كيف يمكنني مساعدتك اليوم؟"
                    : "Welcome to Al-Hayat Hospital 👋\nI'm the smart assistant for Al-Hayat Hospital. I can help you with:\n\n" +
                      "• Booking appointments and finding suitable doctors\n" +
                      "• Department and specialization information\n" +
                      "• Bill and payment inquiries\n" +
                      "• Medicine and pharmacy information\n" +
                      "• Emergency services\n" +
                      "• Working hours and hospital location\n" +
                      "• Insurance information\n" +
                      "• Treatment suggestions based on symptoms\n\n" +
                      "How can I assist you today?",
                Suggestions = new List<string> 
                { 
                    isArabic ? "احجز موعد" : "Book appointment",
                    isArabic ? "ابحث عن طبيب" : "Find doctor",
                    isArabic ? "أعاني من أعراض" : "I have symptoms",
                    isArabic ? "عرض الأقسام" : "View departments"
                }
            };
        }

        private ChatbotResponseDto GetDefaultResponse(bool isArabic)
        {
            return new ChatbotResponseDto
            {
                Response = isArabic
                    ? "عذراً، لم أفهم تماماً. يمكنني مساعدتك في:\n\n" +
                      "• حجز المواعيد\n" +
                      "• البحث عن الأطباء والتخصصات\n" +
                      "• معلومات الأقسام\n" +
                      "• استفسارات الفواتير\n" +
                      "• توفر الأدوية\n" +
                      "• خدمات الطوارئ\n" +
                      "• ساعات العمل والموقع\n" +
                      "• اقتراحات العلاج\n\n" +
                      "يرجى إعادة صياغة سؤالك أو اختر من الاقتراحات أدناه."
                    : "I'm sorry, I didn't quite understand that. I can help you with:\n\n" +
                      "• Booking appointments\n" +
                      "• Finding doctors and specializations\n" +
                      "• Department information\n" +
                      "• Bill inquiries\n" +
                      "• Medicine availability\n" +
                      "• Emergency services\n" +
                      "• Working hours and location\n" +
                      "• Treatment suggestions\n\n" +
                      "Please try rephrasing your question or choose from the suggestions below.",
                Suggestions = new List<string> 
                { 
                    isArabic ? "احجز موعد" : "Book appointment",
                    isArabic ? "ابحث عن طبيب" : "Find doctor",
                    isArabic ? "أعاني من أعراض" : "I have symptoms",
                    isArabic ? "ساعات العمل" : "Working hours"
                }
            };
        }

        private bool ContainsKeywords(string message, string[] keywords)
        {
            return keywords.Any(keyword => message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<string> BuildContextForAI()
        {
            try
            {
                var doctors = await _context.Doctors
                    .Where(d => d.IsActive)
                    .Take(10)
                    .Select(d => $"{d.FirstName} {d.LastName} - {d.Specialization}")
                    .ToListAsync();

                var departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => d.Name)
                    .ToListAsync();

                var availableDoctors = await _context.Doctors
                    .Where(d => d.IsActive && d.IsAvailable)
                    .CountAsync();

                return $"معلومات المستشفى:\n" +
                       $"- عدد الأطباء المتاحين: {availableDoctors}\n" +
                       $"- الأقسام المتاحة: {string.Join(", ", departments)}\n" +
                       $"- بعض الأطباء: {string.Join(", ", doctors)}\n" +
                       $"- ساعات العمل: 8 صباحاً - 8 مساءً\n" +
                       $"- الطوارئ: متاحة 24/7\n" +
                       $"- رقم الطوارئ: 123";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building context for AI");
                return "مستشفى متكامل يقدم خدمات طبية شاملة";
            }
        }

        private List<string> GetDefaultSuggestions(bool isArabic)
        {
            if (isArabic)
            {
                return new List<string> 
                { 
                    "احجز موعد", 
                    "ابحث عن طبيب", 
                    "أعاني من أعراض",
                    "معلومات الأقسام"
                };
            }
            return new List<string> 
            { 
                "Book appointment", 
                "Find doctor", 
                "I have symptoms",
                "Department information"
            };
        }
    }
}
