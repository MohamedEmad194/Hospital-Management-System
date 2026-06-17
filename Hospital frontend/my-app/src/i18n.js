import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

const resources = {
  en: {
    translation: {
      common: {
        appTitle: 'HMS Admin',
        logout: 'Logout',
        login: 'Login',
        createAccount: 'Create Account',
        language: 'Language',
        english: 'English',
        arabic: 'العربية',
        loading: 'Loading…',
        name: 'Name',
        email: 'Email',
        phone: 'Phone',
        department: 'Department',
        nationalId: 'National ID',
        search: 'Search',
        add: 'Add',
        yes: 'Yes',
        no: 'No',
        myAppointments: 'My Appointments',
        myBills: 'My Bills',
        unit: 'Unit',
        currency: 'EGP',
        select: 'Select...',
        pageMeta: 'Page {{page}} of {{totalPages}} · {{total}} total',
        noResults: 'No results found',
        previousPage: 'Previous page',
        nextPage: 'Next page',
        roles: {
          admin: 'Administrator',
          doctor: 'Doctor',
          nurse: 'Nurse',
          pharmacist: 'Pharmacist',
          staff: 'Staff',
          patient: 'Patient',
          guest: 'Guest'
        }
      },
      sidebar: {
        dashboard: 'Dashboard',
        patients: 'Patients',
        doctors: 'Doctors',
        appointments: 'Appointments',
        bills: 'Bills',
        departments: 'Departments',
        rooms: 'Rooms',
        medicines: 'Medicines',
        nursing: 'Nursing',
        chatbot: 'Chat Assistant',
        xrayAi: 'X-Ray AI',
        reports: 'My Medical Reports'
      },
      login: {
        brand: 'HMS',
        welcome: 'Welcome back',
        subtitle: 'Sign in to continue',
        email: 'Email',
        password: 'Password',
        signIn: 'Sign In',
        signingIn: 'Signing in…',
        failed: 'Login failed',
        createAccount: 'Create Account',
        role: 'Login As',
        roles: {
          admin: 'Administrator',
          doctor: 'Doctor',
          patient: 'Patient',
          nurse: 'Nurse'
        },
        networkError: 'Cannot connect to server. Please check if the backend is running.',
        sslError: 'SSL certificate error. Please accept the certificate in your browser.',
        errors: {
          invalidCredentials: 'Incorrect email or password',
          serverConnection: 'Connection error. Make sure the server is running.',
          sslCertificate: 'SSL certificate error',
          accountNotFound: 'No login account for this email',
          accountHint: 'Check your email or create a new account',
          generic: 'An error occurred during login'
        },
        passwordReset: {
          forgot: 'Forgot password?',
          title: 'Reset Password',
          placeholder: 'Enter your email',
          send: 'Send reset link',
          sending: 'Sending…',
          emptyEmail: 'Please enter your email to send a reset link.',
          sent: 'A reset link has been sent to your email if the account exists.',
          failed: 'Could not send reset link. Try again later.'
        }
      },
      register: {
        errors: {
          failed: 'Registration failed',
          emailTaken: 'This email is already in use',
          invalidData: 'Invalid data submitted',
          generic: 'An error occurred during registration'
        }
      },
      xrayAi: {
        title: 'Chest X-ray AI',
        subtitle: 'Local Qwen2.5-VL model via FastAPI (Model/project.py). First analysis may take several minutes while the model loads.',
        disclaimer: 'Assistive only — not a medical diagnosis. Always review with a qualified radiologist.',
        statusReady: 'X-Ray AI service is online and model is loaded.',
        statusReachable: 'Service is online; model loads on first analysis.',
        statusOffline: 'X-Ray AI service is offline. Run Model/run.ps1 then refresh.',
        dropTitle: 'Drop an X-ray image here or click to browse',
        dropHint: 'PNG, JPEG, WebP or BMP — max 20 MB',
        promptLabel: 'Optional instructions',
        promptPlaceholder: 'e.g. Summarize findings and impression…',
        analyze: 'Analyze',
        analyzing: 'Analyzing…',
        clear: 'Clear',
        reportTitle: 'Report',
        waitModel: 'Running vision model — this can take several minutes on first run…',
        errors: {
          notImage: 'Please choose an image file.',
          noFile: 'Please upload an image first.',
          serviceDown: 'Start the FastAPI service: cd Model && .\\run.ps1',
          generic: 'Analysis failed. Check API logs and try again.'
        }
      },
      dashboard: {
        title: 'Dashboard',
        hospitalName: 'Al Hayah',
        subtitle: 'Modern hospital management at a glance',
        description: 'Advanced Hospital Management System - Premium Healthcare',
        bookAppointment: 'Book Appointment Now',
        statistics: 'Hospital Statistics',
        statisticsDesc: 'Comprehensive overview of hospital performance, services and detailed statistics',
        quickActions: 'Quick Actions',
        quickActionsDesc: 'Quick access to essential tasks',
        addPatient: 'Add Patient',
        addPatientDesc: 'Register new patient',
        addDoctor: 'Add Doctor',
        addDoctorDesc: 'Add doctor to hospital',
        bookAppointmentAction: 'Book Appointment',
        bookAppointmentDesc: 'Book new appointment',
        addBill: 'Add Bill',
        addBillDesc: 'Create new bill',
        doctors: 'Doctors',
        departments: 'Departments',
        appointments: 'Appointments',
        staffPanel: {
          welcome: 'Welcome back, {{name}}',
          today: 'Today, {{date}}',
          subtitle: 'Here is what is happening at the hospital right now.',
          todaysAppointments: 'Today\'s Appointments',
          availableRooms: 'Available Rooms',
          totalPatients: 'Total Patients',
          todaysBills: 'Bills Issued Today',
          viewAll: 'View all',
          manage: 'Manage'
        },
        errors: {
          authRequired: 'Please login to view statistics',
          backendDown: 'Cannot connect to server. Please ensure the backend is running.',
          serverError: 'Server error. Please try again later.',
          backendHint: 'Please ensure the backend is running on http://localhost:5230',
          retry: 'Retry'
        },
        features: {
          quickCare: {
            title: 'Quick Care',
            desc: 'Fast and efficient medical services'
          },
          highPrecision: {
            title: 'High Precision',
            desc: 'Accurate diagnosis and specialized treatment'
          },
          comprehensiveCare: {
            title: 'Comprehensive Care',
            desc: 'We provide you with the best healthcare'
          }
        },
        nursing: {
          badge: 'Nursing Excellence',
          title: 'Nursing Care & Recovery Hub',
          subtitle: 'Integrated nursing teams covering ICU, emergency, wards and home follow-ups 24/7.',
          cards: {
            criticalCare: {
              title: 'Critical Care Units',
              desc: 'Dual-certified nurses with real-time monitoring skills for ICU and step-down beds.'
            },
            patientEducation: {
              title: 'Patient Education',
              desc: 'Bedside teaching plans that improve medication adherence and safe recovery.'
            },
            homeFollowup: {
              title: 'Home Follow-ups',
              desc: 'Coordinated discharge calls and in-home visits for high-risk cases.'
            },
            digitalMonitoring: {
              title: 'Digital Monitoring',
              desc: 'Smart dashboards to track vitals, falls risk and complication alerts.'
            }
          },
          metrics: {
            coverage: { label: 'Coverage', value: '24/7' },
            ratio: { label: 'Nurse-to-patient ratio', value: '1 : 4' },
            satisfaction: { label: 'Patient satisfaction', value: '95%' },
            lead: { label: 'Nursing director', value: 'Sarah Youssef' }
          },
          timeline: {
            assessment: {
              title: 'Morning assessment',
              desc: 'Vitals, infusion checks and interdisciplinary briefings.'
            },
            rounds: {
              title: 'Hourly safety rounds',
              desc: 'Mobility support, wound care and comfort updates for families.'
            },
            reporting: {
              title: 'Evening reporting',
              desc: 'Handover with digital documentation and escalation planning.'
            }
          },
          cta: {
            title: 'Need a specialized nursing plan?',
            desc: 'Our leadership team can tailor bedside, home and tele-health nursing coverage for your case.',
            primary: 'Meet nursing team',
            secondary: 'Book nursing consult'
          }
        },
        about: {
          badge: 'Inside Al Hayah Hospital',
          title: 'Spaces built for calm, precision and flow',
          description: 'Take a visual walk-through of the real wards, reception and theatres our patients use every day. Every photo in this gallery was captured after the 2025 refurbishment, so what you see is exactly what your family will experience on-site.',
          areas: {
            wards: {
              title: 'Open recovery ward',
              desc: 'Blue duvets, pastel privacy curtains and daylight from both sides keep the six-bed bay quiet yet fully observable.',
              detail: 'Overhead rails let nurses glide monitoring gear to any bed without moving the patient.',
              metricLabel: 'Ward B3',
              metricLabelShort: 'beds ready',
              metricValue: '32'
            },
            reception: {
              title: 'Reception & triage lobby',
              desc: 'The curved welcome desk and bilingual signage guide arrivals to elevators, blood bank, neonatal and adult therapy suites.',
              detail: 'Direct sightlines to the lift core help attendants fast-track urgent cases.',
              metricLabel: 'Arrival lobby',
              metricLabelShort: 'service desks',
              metricValue: '3'
            },
            privateSuite: {
              title: 'Private inpatient suite',
              desc: 'Mint bedding, a sleeper sofa and discreet headwall services let long-stay patients heal alongside their families.',
              detail: 'The integrated panel hides medical gases, comms and nurse-call buttons for a hotel-like feel.',
              metricLabel: 'Family suite',
              metricLabelShort: 'comfort zones',
              metricValue: '2'
            },
            surgerySuite: {
              title: 'Hybrid operating theatre',
              desc: 'Dual surgical lights, imaging displays and modular anesthesia stations keep the lead surgeon within arm’s reach of every tool.',
              detail: 'Laminar airflow tiles and antimicrobial flooring underpin our infection-control protocol.',
              metricLabel: 'Hybrid OR',
              metricLabelShort: 'ceiling booms',
              metricValue: '4'
            },
            minimalOr: {
              title: 'Day-surgery theatre',
              desc: 'An ultra-white, compact suite supports minimally invasive cases with fast turnover and easy cleaning.',
              detail: 'Mobile trolleys let the team reconfigure the room between specialties in minutes.',
              metricLabel: 'Day surgery',
              metricLabelShort: 'avg. turnover',
              metricValue: '18 min'
            },
            corridor: {
              title: 'Critical care corridor',
              desc: 'Wide hallways with stretcher lay-bys and mounted wayfinding keep traffic flowing toward ICU and specialty rooms.',
              detail: 'Crash carts dock along the wall so nothing blocks the emergency path.',
              metricLabel: 'Critical path',
              metricLabelShort: 'm coverage',
              metricValue: '60'
            },
            exterior: {
              title: 'Main campus arrival',
              desc: 'The brick façade and sweeping porte-cochère shelter ambulances and family vehicles during drop-off.',
              detail: 'Separated lanes mean emergency crews never get stuck behind visitors.',
              metricLabel: 'Main campus',
              metricLabelShort: 'acre site',
              metricValue: '12'
            }
          }
        },
        items: {
          patients: 'Patients',
          doctors: 'Doctors',
          appointments: 'Appointments',
          departments: 'Departments',
          rooms: 'Rooms',
          medicines: 'Medicines',
          bills: 'Bills',
          pendingAppointments: 'Pending Appointments',
          completedAppointments: 'Completed Appointments',
          overdueBills: 'Overdue Bills',
          lowStockMedicines: 'Low Stock Medicines'
        },
        highlights: {
          revenueTotal: 'Total Revenue',
          revenueTotalHint: 'Lifetime paid bills',
          revenueMonth: 'This Month',
          revenueMonthHint: 'Collected since month start',
          revenuePending: 'Pending Revenue',
          revenuePendingHint: 'Outstanding on open bills',
          todayAppointments: "Today's Appointments",
          todayAppointmentsHint: 'Scheduled for today'
        },
        insights: {
          title: 'Performance Insights',
          desc: 'Revenue, today\'s activity, status mix and a 6-month appointment trend.',
          statusTitle: 'Appointments by status',
          statusSubtitle: 'Distribution across the full schedule',
          topDeptsTitle: 'Top departments',
          topDeptsSubtitle: 'Ranked by total appointments',
          trendTitle: 'Appointment trend (last 6 months)',
          trendSubtitle: 'Volume by month',
          todayTitle: "Today's pulse",
          todaySubtitle: 'Live snapshot of activity',
          newPatients: 'New patients',
          billsIssued: 'Bills issued',
          empty: 'No data available yet.'
        },
        failed: 'Failed to load stats',
        loading: 'Loading dashboard…'
      },
      patients: {
        searchPlaceholder: 'Search patients by name, email, phone, national id',
        loading: 'Loading patients…',
        failed: 'Failed to load patients',
        table: {
          name: 'Name',
          email: 'Email',
          phone: 'Phone',
          nationalId: 'National ID'
        }
      },
      doctors: {
        searchPlaceholder: 'Search doctors by name, email, license, department',
        loading: 'Loading doctors…',
        failed: 'Failed to load doctors',
        table: {
          name: 'Name',
          email: 'Email',
          phone: 'Phone',
          department: 'Department'
        },
        labels: {
          email: 'Email',
          phone: 'Phone',
          department: 'Department',
          specialization: 'Specialization'
        }
      },
      appointments: {
        title: 'Appointments',
        addButton: 'Add Appointment',
        loading: 'Loading appointments…',
        failed: 'Failed to load appointments',
        table: {
          patient: 'Patient',
          doctor: 'Doctor',
          date: 'Date',
          time: 'Time',
          status: 'Status'
        }
      },
      bills: {
        title: 'Bills',
        addButton: 'Add Bill',
        loading: 'Loading bills…',
        failed: 'Failed to load bills',
        summary: {
          overdue: 'Overdue',
          outstanding: 'Outstanding'
        },
        table: {
          patient: 'Patient',
          total: 'Total',
          paid: 'Paid',
          remaining: 'Remaining',
          status: 'Status'
        }
      },
      departments: {
        title: 'Departments',
        addButton: 'Add Department',
        loading: 'Loading departments…',
        failed: 'Failed to load departments',
        table: {
          name: 'Name',
          description: 'Description'
        },
        labels: {
          description: 'Description',
          headOfDepartment: 'Head of Department',
          phone: 'Phone',
          location: 'Location'
        }
      },
      medicines: {
        title: 'Medicines',
        addButton: 'Add Medicine',
        loading: 'Loading medicines…',
        failed: 'Failed to load medicines',
        table: {
          name: 'Name',
          stock: 'Stock',
          minimumStock: 'Minimum Stock'
        },
        labels: {
          genericName: 'Generic Name',
          strength: 'Strength',
          availableQuantity: 'Available Quantity',
          minimumStock: 'Minimum Stock',
          price: 'Price',
          unit: 'Unit',
          lowStockWarning: 'Warning: Low stock'
        }
      },
      rooms: {
        title: 'Rooms',
        addButton: 'Add Room',
        loading: 'Loading rooms…',
        failed: 'Failed to load rooms',
        table: {
          room: 'Room',
          type: 'Type',
          department: 'Department',
          available: 'Available'
        },
        labels: {
          type: 'Type',
          department: 'Department',
          floor: 'Floor',
          status: 'Status',
          available: 'Available',
          unavailable: 'Unavailable'
        },
        types: {
          Consultation: 'Consultation',
          ICU: 'ICU',
          Surgery: 'Surgery',
          Ward: 'Ward',
          Emergency: 'Emergency',
          Radiology: 'Radiology'
        }
      },
      addPatient: {
        title: 'Add Patient',
        failed: 'Failed to create patient',
        fields: {
          firstName: 'First Name',
          lastName: 'Last Name',
          nationalId: 'National ID',
          email: 'Email',
          phone: 'Phone',
          dateOfBirth: 'Date of Birth',
          gender: 'Gender',
          male: 'Male',
          female: 'Female',
          address: 'Address'
        },
        actions: {
          save: 'Save Patient',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      addDoctor: {
        title: 'Add Doctor',
        failed: 'Failed to create doctor',
        fields: {
          firstName: 'First Name',
          lastName: 'Last Name',
          nationalId: 'National ID',
          email: 'Email',
          phone: 'Phone',
          dateOfBirth: 'Date of Birth',
          gender: 'Gender',
          male: 'Male',
          female: 'Female',
          address: 'Address',
          licenseNumber: 'License Number',
          specialization: 'Specialization',
          yearsOfExperience: 'Years of Experience',
          consultationFee: 'Consultation Fee',
          workingStart: 'Working Start',
          workingEnd: 'Working End',
          department: 'Department',
          selectDepartment: 'Select department'
        },
        actions: {
          save: 'Save Doctor',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      addDepartment: {
        title: 'Add Department',
        failed: 'Failed to create department',
        fields: {
          name: 'Name',
          description: 'Description'
        },
        actions: {
          save: 'Save Department',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      addAppointment: {
        title: 'Add Appointment',
        failed: 'Failed to create appointment',
        fields: {
          patient: 'Patient',
          selectPatient: 'Select patient',
          doctor: 'Doctor',
          selectDoctor: 'Select doctor',
          date: 'Date',
          time: 'Time',
          reason: 'Reason',
          notes: 'Notes',
          room: 'Room (Optional)',
          selectRoom: 'Select room (optional)'
        },
        actions: {
          save: 'Save Appointment',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      addBill: {
        title: 'Add Bill',
        failed: 'Failed to create bill',
        fields: {
          patient: 'Patient',
          selectPatient: 'Select patient',
          billDate: 'Bill Date',
          dueDate: 'Due Date',
          insuranceProvider: 'Insurance Provider',
          insuranceNumber: 'Insurance Number',
          insuranceCoverage: 'Insurance Coverage'
        },
        items: {
          title: 'Items',
          description: 'Description',
          qty: 'Qty',
          unitPrice: 'Unit Price',
          addItem: 'Add Item',
          remove: 'Remove'
        },
        actions: {
          save: 'Save Bill',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      addMedicine: {
        title: 'Add Medicine',
        failed: 'Failed to create medicine',
        fields: {
          name: 'Name',
          genericName: 'Generic Name',
          dosageForm: 'Dosage Form',
          strength: 'Strength',
          manufacturer: 'Manufacturer',
          price: 'Price',
          stockQuantity: 'Stock Quantity',
          minimumStock: 'Minimum Stock',
          unit: 'Unit',
          expiryDate: 'Expiry Date',
          batchNumber: 'Batch Number',
          requiresPrescription: 'Requires Prescription',
          yes: 'Yes',
          no: 'No'
        },
        actions: {
          save: 'Save Medicine',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      addRoom: {
        title: 'Add Room',
        failed: 'Failed to create room',
        fields: {
          roomNumber: 'Room Number',
          roomType: 'Room Type',
          floor: 'Floor',
          building: 'Building',
          description: 'Description',
          capacity: 'Capacity',
          hourlyRate: 'Hourly Rate',
          departmentId: 'Department ID',
          isAvailable: 'Available'
        },
        actions: {
          save: 'Save Room',
          saving: 'Saving…',
          cancel: 'Cancel'
        }
      },
      chatbot: {
        title: 'Hospital Assistant',
        subtitle: 'Ask me anything about our services',
        welcome: 'Hello! I\'m your hospital assistant. I can help you with:\n- Booking appointments\n- Finding doctors\n- Department information\n- Bill inquiries\n- Medicine availability\n- Emergency services\n- Hospital hours and location\n\nHow can I assist you today?',
        placeholder: 'Type your message here...',
        send: 'Send',
        sending: 'Sending...',
        error: 'Sorry, I encountered an error. Please try again or contact our support team.',
        noResponse: 'I didn\'t receive a response. Please try again.'
      },
      nursingPage: {
        badge: 'Trusted nursing care',
        title: 'Integrated Nursing Department',
        subtitle: 'Continuous bedside and home coverage with advanced monitoring, education and family support.',
        cta: {
          primary: 'Book nursing consult',
          secondary: 'Browse departments'
        },
        metrics: {
          coverage: { label: 'Coverage', value: '24/7' },
          response: { label: 'Rapid response', value: '< 4 min' },
          satisfaction: { label: 'Family satisfaction', value: '97%' }
        },
        promise: {
          label: 'Care promise',
          title: 'Human-centered care with smart tools',
          desc: 'Our shift leaders coordinate with physicians, pharmacists and physiotherapists to make sure every patient receives personalized care pathways.'
        },
        pillars: {
          compassion: {
            title: 'Compassion',
            desc: 'Warm bedside presence, active listening and cultural sensitivity.'
          },
          coverage: {
            title: '24/7 Coverage',
            desc: 'Rotating teams covering ICU, emergency, wards and home services.'
          },
          expertise: {
            title: 'Clinical expertise',
            desc: 'Evidence-based protocols, infusion therapy and specialized wound care.'
          },
          familySupport: {
            title: 'Family support',
            desc: 'Clear updates, education sessions and discharge coaching.'
          }
        },
        programs: {
          label: 'Programs',
          title: 'Specialized nursing programs',
          desc: 'Each program has certified nurses, digital documentation and clear escalation ladders.',
          emergency: {
            title: 'Emergency readiness',
            desc: 'Rapid triage teams with trauma and ACLS certifications.'
          },
          icu: {
            title: 'ICU precision',
            desc: 'Advanced ventilator management and infection control monitoring.'
          },
          pediatrics: {
            title: 'Pediatric comfort',
            desc: 'Family-centered rooms, pain distraction kits and growth tracking.'
          },
          homeCare: {
            title: 'Home care transition',
            desc: 'Tele-nursing follow ups, medication reconciliation and rehab coordination.'
          }
        },
        support: {
          title: 'Leadership & support channels',
          desc: 'Clinical nurse educators, quality officers and case managers stay connected with each shift.',
          rounds: { label: 'Daily leadership rounds', value: '3+' },
          consultations: { label: 'Bedside consults', value: '18 / day' },
          training: { label: 'Upskilling workshops', value: '12 / month' },
          ctaTitle: 'Let us design your care journey',
          ctaDesc: 'Share your case details and we will recommend the ideal nursing mix.',
          ctaButton: 'Talk to nursing lead'
        },
        dataset: {
          label: 'Nursing dataset',
          title: 'Bedside & remote coverage dataset',
          desc: 'Explore staffing, coverage windows and focus areas for every nursing unit. Export the raw dataset for analytics or reporting.',
          searchPlaceholder: 'Search by unit, wing or lead nurse…',
          download: 'Download CSV dataset',
          columns: {
            unit: 'Unit',
            wing: 'Wing / location',
            lead: 'Lead nurse',
            nurses: 'Nurses',
            coverage: 'Coverage',
            focus: 'Clinical focus'
          },
          emptyState: 'No units match “{{query}}”. Try another keyword.'
        }
      }
    }
  },
  ar: {
    translation: {
      common: {
        appTitle: 'إدارة المستشفى',
        logout: 'تسجيل الخروج',
        login: 'تسجيل دخول',
        createAccount: 'إنشاء حساب',
        language: 'اللغة',
        english: 'English',
        arabic: 'العربية',
        loading: 'جارٍ التحميل…',
        name: 'الاسم',
        email: 'البريد الإلكتروني',
        phone: 'الهاتف',
        department: 'القسم',
        nationalId: 'الرقم القومي',
        search: 'بحث',
        add: 'إضافة',
        yes: 'نعم',
        no: 'لا',
        myAppointments: 'مواعيدي',
        myBills: 'فواتيري',
        unit: 'وحدة',
        currency: 'جنيه',
        select: 'اختر...',
        pageMeta: 'صفحة {{page}} من {{totalPages}} · {{total}} إجمالاً',
        noResults: 'لا توجد نتائج',
        previousPage: 'الصفحة السابقة',
        nextPage: 'الصفحة التالية',
        roles: {
          admin: 'مدير النظام',
          doctor: 'طبيب',
          nurse: 'ممرض/ممرضة',
          pharmacist: 'صيدلي',
          staff: 'موظف',
          patient: 'مريض',
          guest: 'زائر'
        }
      },
      sidebar: {
        dashboard: 'الرئيسية',
        patients: 'المرضى',
        doctors: 'الأطباء',
        appointments: 'المواعيد',
        bills: 'الفواتير',
        departments: 'الأقسام',
        rooms: 'الغرف',
        medicines: 'الأدوية',
        nursing: 'التمريض',
        chatbot: 'المساعد الذكي',
        xrayAi: 'أشعة بالذكاء الاصطناعي',
        reports: 'تقاريري الطبية'
      },
      login: {
        brand: 'HMS',
        welcome: 'مرحباً بعودتك',
        subtitle: 'سجل الدخول للمتابعة',
        email: 'البريد الإلكتروني',
        password: 'كلمة المرور',
        signIn: 'تسجيل الدخول',
        signingIn: 'جارٍ التسجيل…',
        failed: 'فشل التسجيل',
        createAccount: 'إنشاء حساب',
        role: 'تسجيل الدخول كـ',
        roles: {
          admin: 'مدير النظام',
          doctor: 'طبيب',
          patient: 'مريض',
          nurse: 'ممرض/ممرضة'
        },
        networkError: 'لا يمكن الاتصال بالخادم. يرجى التأكد من عمل الخلفية.',
        sslError: 'خطأ شهادة SSL. يرجى قبول الشهادة في متصفحك.',
        errors: {
          invalidCredentials: 'البريد الإلكتروني أو كلمة المرور غير صحيحة',
          serverConnection: 'خطأ في الاتصال بالخادم. تأكد من تشغيل الخادم',
          sslCertificate: 'خطأ في شهادة SSL',
          accountNotFound: 'لا يوجد حساب دخول لهذا البريد',
          accountHint: 'تأكد من البريد، أو أنشئ حساباً جديداً',
          generic: 'حدث خطأ أثناء تسجيل الدخول'
        },
        passwordReset: {
          forgot: 'نسيت كلمة المرور؟',
          title: 'إعادة تعيين كلمة المرور',
          placeholder: 'أدخل بريدك الإلكتروني',
          send: 'أرسل رابط إعادة التعيين',
          sending: 'جارٍ الإرسال...',
          emptyEmail: 'رجاءً أدخل البريد الإلكتروني لإرسال رابط إعادة التعيين.',
          sent: 'تم إرسال رابط إعادة التعيين إلى بريدك إذا كان الحساب موجوداً.',
          failed: 'تعذر إرسال رابط إعادة التعيين. حاول لاحقاً.'
        }
      },
      register: {
        errors: {
          failed: 'فشل التسجيل',
          emailTaken: 'البريد الإلكتروني مستخدم بالفعل',
          invalidData: 'خطأ في البيانات المدخلة',
          generic: 'حدث خطأ أثناء التسجيل'
        }
      },
      xrayAi: {
        title: 'تحليل أشعة الصدر',
        subtitle: 'نموذج Qwen2.5-VL محلي عبر FastAPI (Model/project.py). أول تحليل قد يستغرق دقائق لتحميل النموذج.',
        disclaimer: 'للمساعدة فقط — وليس تشخيصاً طبياً. راجع النتائج مع أخصائي أشعة.',
        statusReady: 'خدمة الأشعة متصلة والنموذج جاهز.',
        statusReachable: 'الخدمة متصلة؛ النموذج يُحمّل عند أول تحليل.',
        statusOffline: 'خدمة الأشعة غير متصلة. شغّل Model/run.ps1 ثم حدّث الصفحة.',
        dropTitle: 'اسحب صورة الأشعة هنا أو انقر للاختيار',
        dropHint: 'PNG أو JPEG أو WebP أو BMP — بحد أقصى 20 ميجابايت',
        promptLabel: 'تعليمات اختيارية',
        promptPlaceholder: 'مثال: لخّص النتائج والانطباع…',
        analyze: 'تحليل',
        analyzing: 'جاري التحليل…',
        clear: 'مسح',
        reportTitle: 'التقرير',
        waitModel: 'تشغيل نموذج الرؤية — قد يستغرق عدة دقائق في أول مرة…',
        errors: {
          notImage: 'يرجى اختيار ملف صورة.',
          noFile: 'يرجى رفع صورة أولاً.',
          serviceDown: 'شغّل خدمة FastAPI: cd Model ثم .\\run.ps1',
          generic: 'فشل التحليل. راجع سجلات الـ API وحاول مرة أخرى.'
        }
      },
      dashboard: {
        title: 'لوحة المتابعة',
        hospitalName: 'الحياة',
        subtitle: 'إدارة المستشفى الحديثة بنظرة واحدة',
        description: 'نظام إدارة المستشفى المتطور - رعاية صحية متميزة',
        bookAppointment: 'احجز موعدك الآن',
        statistics: 'إحصائيات المستشفى',
        statisticsDesc: 'نظرة شاملة على أداء المستشفى وخدماته وإحصائيات مفصلة',
        quickActions: 'الإجراءات السريعة',
        quickActionsDesc: 'وصول سريع إلى المهام الأساسية',
        addPatient: 'إضافة مريض',
        addPatientDesc: 'تسجيل مريض جديد',
        addDoctor: 'إضافة طبيب',
        addDoctorDesc: 'إضافة طبيب للمستشفى',
        bookAppointmentAction: 'حجز موعد',
        bookAppointmentDesc: 'حجز موعد جديد',
        addBill: 'إضافة فاتورة',
        addBillDesc: 'إنشاء فاتورة جديدة',
        doctors: 'أطباء',
        departments: 'أقسام',
        appointments: 'مواعيد',
        staffPanel: {
          welcome: 'أهلاً بعودتك، {{name}}',
          today: 'اليوم، {{date}}',
          subtitle: 'دي حالة المستشفى دلوقتي.',
          todaysAppointments: 'مواعيد اليوم',
          availableRooms: 'الغرف المتاحة',
          totalPatients: 'إجمالي المرضى',
          todaysBills: 'فواتير صدرت اليوم',
          viewAll: 'عرض الكل',
          manage: 'إدارة'
        },
        errors: {
          authRequired: 'يجب تسجيل الدخول لعرض الإحصائيات',
          backendDown: 'لا يمكن الاتصال بالخادم. تأكد من تشغيل الباك إند.',
          serverError: 'خطأ في الخادم. يرجى المحاولة لاحقاً.',
          backendHint: 'تأكد من أن الباك إند يعمل على http://localhost:5230',
          retry: 'إعادة المحاولة'
        },
        features: {
          quickCare: {
            title: 'رعاية سريعة',
            desc: 'خدمات طبية سريعة وفعالة'
          },
          highPrecision: {
            title: 'دقة عالية',
            desc: 'تشخيص دقيق وعلاج متخصص'
          },
          comprehensiveCare: {
            title: 'رعاية شاملة',
            desc: 'نوفر لك أفضل رعاية صحية'
          }
        },
        about: {
          badge: 'جولة داخل المستشفى',
          title: 'مساحات صُممت للهدوء والدقة والانسيابية',
          description: 'اصطحب عائلتك في جولة بصرية داخل الأجنحة والاستقبال وغرف العمليات الفعلية التي نستخدمها يومياً. التُقطت هذه الصور بعد تحديث 2025، لذا فالتجربة التي تراها هي نفسها التي سنوفرها لك على أرض الواقع.',
          areas: {
            wards: {
              title: 'جناح تعافٍ مفتوح',
              desc: 'أغطية زرقاء وستائر خصوصية وردية وإضاءة طبيعية من الجانبين تبقي الجناح الهادئ قابلاً للمراقبة في نفس الوقت.',
              detail: 'السكك العلوية تمكّن الممرضات من تحريك أجهزة المتابعة لأي سرير دون إزعاج المريض.',
              metricLabel: 'جناح B3',
              metricLabelShort: 'سريراً جاهزاً',
              metricValue: '32'
            },
            reception: {
              title: 'ردهة الاستقبال والفرز',
              desc: 'مكتب استقبال منحني ولافتات ثنائية اللغة توجه الزوار نحو المصاعد وبنك الدم والرعاية المتخصصة.',
              detail: 'خط الرؤية المباشر إلى نواة المصاعد يساعد الفريق في تسريع استقبال الحالات العاجلة.',
              metricLabel: 'ردهة الوصول',
              metricLabelShort: 'مكاتب خدمة',
              metricValue: '3'
            },
            privateSuite: {
              title: 'جناح إقامة خاص',
              desc: 'مفارش بلون النعناع وأريكة مبيت وخدمات رأس سرير مخفية تمنح المرضى المقيمين راحة فندقية مع بقاء الأسرة بجانبهم.',
              detail: 'لوحة رأسية مدمجة تخفي خطوط الغازات الطبية والاتصال وجرس النداء.',
              metricLabel: 'جناح العائلة',
              metricLabelShort: 'مناطق راحة',
              metricValue: '2'
            },
            surgerySuite: {
              title: 'غرفة عمليات هجينة',
              desc: 'مصابيح جراحية مزدوجة وشاشات تصوير ووحدات تخدير معيارية تُبقي الجراح قائد الفريق في متناول كل أداة.',
              detail: 'ألواح تدفق هوائي أرضي وأرضيات مضادة للميكروبات تعزز مكافحة العدوى.',
              metricLabel: 'غرفة هجينة',
              metricLabelShort: 'أذرع سقفية',
              metricValue: '4'
            },
            minimalOr: {
              title: 'غرفة جراحات اليوم الواحد',
              desc: 'مساحة فائقة البياض ومدمجة تدعم العمليات طفيفة التوغل مع تبديل سريع وسهل التنظيف.',
              detail: 'العربات المتحركة تسمح بإعادة تهيئة الغرفة بين التخصصات في دقائق.',
              metricLabel: 'جراحات اليوم',
              metricLabelShort: 'متوسط التبديل',
              metricValue: '18 د'
            },
            corridor: {
              title: 'ممر الرعاية الحرجة',
              desc: 'ممرات واسعة مع نقاط توقف للنقالات ولافتات معلقة تُبقي الحركة سالسة نحو العناية المركزة.',
              detail: 'تُثبت عربات الطوارئ على الجوانب حتى يبقى المسار مفتوحاً دائماً.',
              metricLabel: 'مسار حرج',
              metricLabelShort: 'تغطية م',
              metricValue: '60'
            },
            exterior: {
              title: 'المدخل الرئيسي للحرم',
              desc: 'واجهة من الطوب ومظلة كبيرة تؤمن إسعاف المرضى والعائلات أثناء النزول.',
              detail: 'مسارات منفصلة تمنع تداخل سيارات الزوار مع سيارات الطوارئ.',
              metricLabel: 'الحرم الرئيسي',
              metricLabelShort: 'مساحة فدان',
              metricValue: '12'
            }
          }
        },
        nursing: {
          badge: 'تميز التمريض',
          title: 'مركز رعاية وتعافي التمريض',
          subtitle: 'فرق تمريض متخصصة تغطي العناية المركزة والطوارئ والأجنحة والمتابعة المنزلية على مدار الساعة.',
          cards: {
            criticalCare: {
              title: 'وحدات العناية الحرجة',
              desc: 'ممرضات معتمدات بمهارات مراقبة فورية لأسرة العناية والمنتصف.'
            },
            patientEducation: {
              title: 'تثقيف المرضى',
              desc: 'خطط تعليمية بجانب السرير تعزز الالتزام بالأدوية والتعافي الآمن.'
            },
            homeFollowup: {
              title: 'متابعة منزلية',
              desc: 'مكالمات خروج وزيارات منزلية منسقة للحالات عالية الخطورة.'
            },
            digitalMonitoring: {
              title: 'مراقبة رقمية',
              desc: 'لوحات ذكية لمتابعة العلامات الحيوية ومخاطر السقوط والتنبيهات.'
            }
          },
          metrics: {
            coverage: { label: 'التغطية', value: '24/7' },
            ratio: { label: 'نسبة الممرض/المريض', value: '1 : 4' },
            satisfaction: { label: 'رضا المرضى', value: '95%' },
            lead: { label: 'مديرة التمريض', value: 'سارة يوسف' }
          },
          timeline: {
            assessment: {
              title: 'تقييم صباحي',
              desc: 'علامات حيوية، مراجعة محاليل واجتماع الفريق الطبي.'
            },
            rounds: {
              title: 'جولات السلامة كل ساعة',
              desc: 'دعم الحركة، العناية بالجروح وتحديث العائلات بالراحة.'
            },
            reporting: {
              title: 'تسليم مسائي',
              desc: 'تسليم رقمي مع خطط تصعيد ومتابعة دقيقة.'
            }
          },
          cta: {
            title: 'تحتاج خطة تمريض متخصصة؟',
            desc: 'فريق القيادة يمكنه تصميم تغطية تمريضية بالمستشفى أو المنزل أو عن بُعد لحالتك.',
            primary: 'تعرف على فريق التمريض',
            secondary: 'احجز استشارة تمريضية'
          }
        },
        items: {
          patients: 'المرضى',
          doctors: 'الأطباء',
          appointments: 'المواعيد',
          departments: 'الأقسام',
          rooms: 'الغرف',
          medicines: 'الأدوية',
          bills: 'الفواتير',
          pendingAppointments: 'مواعيد منتظرة',
          completedAppointments: 'مواعيد مكتملة',
          overdueBills: 'فواتير متأخرة',
          lowStockMedicines: 'أدوية قليلة المخزون'
        },
        highlights: {
          revenueTotal: 'إجمالي الإيرادات',
          revenueTotalHint: 'الفواتير المسددة الكلية',
          revenueMonth: 'إيرادات الشهر',
          revenueMonthHint: 'المحصل منذ بداية الشهر',
          revenuePending: 'الإيرادات المستحقة',
          revenuePendingHint: 'فواتير لم تُسدد بعد',
          todayAppointments: 'مواعيد اليوم',
          todayAppointmentsHint: 'الحجوزات المجدولة اليوم'
        },
        insights: {
          title: 'مؤشرات الأداء',
          desc: 'الإيرادات ونشاط اليوم وتوزيع الحالات واتجاه آخر 6 أشهر.',
          statusTitle: 'المواعيد حسب الحالة',
          statusSubtitle: 'توزيع شامل للمواعيد',
          topDeptsTitle: 'أكثر الأقسام نشاطاً',
          topDeptsSubtitle: 'مرتبة حسب عدد المواعيد',
          trendTitle: 'اتجاه المواعيد (آخر 6 أشهر)',
          trendSubtitle: 'الحجم الشهري',
          todayTitle: 'نبض اليوم',
          todaySubtitle: 'لقطة فورية للنشاط',
          newPatients: 'مرضى جدد',
          billsIssued: 'فواتير صادرة',
          empty: 'لا توجد بيانات حالياً.'
        },
        failed: 'فشل تحميل الإحصاءات',
        loading: 'جارٍ تحميل اللوحة…'
      },
      patients: {
        searchPlaceholder: 'ابحث عن مريض بالاسم، البريد، الهاتف، القومي',
        loading: 'جارٍ تحميل المرضى…',
        failed: 'فشل تحميل المرضى',
        table: {
          name: 'الاسم',
          email: 'البريد',
          phone: 'الهاتف',
          nationalId: 'الرقم القومي'
        }
      },
      doctors: {
        searchPlaceholder: 'ابحث عن طبيب بالاسم، البريد، الترخيص، القسم',
        loading: 'جارٍ تحميل الأطباء…',
        failed: 'فشل تحميل الأطباء',
        table: {
          name: 'الاسم',
          email: 'البريد',
          phone: 'الهاتف',
          department: 'القسم'
        },
        labels: {
          email: 'البريد الإلكتروني',
          phone: 'الهاتف',
          department: 'القسم',
          specialization: 'التخصص'
        }
      },
      appointments: {
        title: 'المواعيد',
        addButton: 'إضافة موعد',
        loading: 'جارٍ تحميل المواعيد…',
        failed: 'فشل تحميل المواعيد',
        table: {
          patient: 'المريض',
          doctor: 'الطبيب',
          date: 'التاريخ',
          time: 'الوقت',
          status: 'الحالة'
        }
      },
      bills: {
        title: 'الفواتير',
        addButton: 'إضافة فاتورة',
        loading: 'جارٍ تحميل الفواتير…',
        failed: 'فشل تحميل الفواتير',
        summary: {
          overdue: 'متأخر',
          outstanding: 'مستحقات'
        },
        table: {
          patient: 'المريض',
          total: 'الإجمالي',
          paid: 'المدفوع',
          remaining: 'المتبقي',
          status: 'الحالة'
        }
      },
      departments: {
        title: 'الأقسام',
        addButton: 'إضافة قسم',
        loading: 'جارٍ تحميل الأقسام…',
        failed: 'فشل تحميل الأقسام',
        table: {
          name: 'الاسم',
          description: 'الوصف'
        },
        labels: {
          description: 'الوصف',
          headOfDepartment: 'رئيس القسم',
          phone: 'الهاتف',
          location: 'الموقع'
        }
      },
      medicines: {
        title: 'الأدوية',
        addButton: 'إضافة دواء',
        loading: 'جارٍ تحميل الأدوية…',
        failed: 'فشل تحميل الأدوية',
        table: {
          name: 'اسم الدواء',
          stock: 'المخزون',
          minimumStock: 'الحد الأدنى للمخزون'
        },
        labels: {
          genericName: 'الاسم العلمي',
          strength: 'التركيز',
          availableQuantity: 'الكمية المتوفرة',
          minimumStock: 'الحد الأدنى',
          price: 'السعر',
          unit: 'وحدة',
          lowStockWarning: 'تنبيه: الكمية منخفضة'
        }
      },
      rooms: {
        title: 'الغرف',
        addButton: 'إضافة غرفة',
        loading: 'جارٍ تحميل الغرف…',
        failed: 'فشل تحميل الغرف',
        table: {
          room: 'الغرفة',
          type: 'النوع',
          department: 'القسم',
          available: 'متاحة'
        },
        labels: {
          type: 'النوع',
          department: 'القسم',
          floor: 'الطابق',
          status: 'الحالة',
          available: 'متاح',
          unavailable: 'غير متاح'
        },
        types: {
          Consultation: 'استشارة',
          ICU: 'العناية المركزة',
          Surgery: 'الجراحة',
          Ward: 'جناح',
          Emergency: 'الطوارئ',
          Radiology: 'الأشعة'
        }
      },
      addPatient: {
        title: 'إضافة مريض',
        failed: 'فشل إنشاء مريض',
        fields: {
          firstName: 'الاسم الأول',
          lastName: 'اللقب',
          nationalId: 'الرقم القومي',
          email: 'البريد الإلكتروني',
          phone: 'الهاتف',
          dateOfBirth: 'تاريخ الميلاد',
          gender: 'الجنس',
          male: 'ذكر',
          female: 'أنثى',
          address: 'العنوان'
        },
        actions: {
          save: 'حفظ المريض',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      addDoctor: {
        title: 'إضافة طبيب',
        failed: 'فشل إنشاء طبيب',
        fields: {
          firstName: 'الاسم الأول',
          lastName: 'اللقب',
          nationalId: 'الرقم القومي',
          email: 'البريد',
          phone: 'الهاتف',
          dateOfBirth: 'تاريخ الميلاد',
          gender: 'الجنس',
          male: 'ذكر',
          female: 'أنثى',
          address: 'العنوان',
          licenseNumber: 'رقم الترخيص',
          specialization: 'التخصص',
          yearsOfExperience: 'سنوات الخبرة',
          consultationFee: 'رسوم الاستشارة',
          workingStart: 'بداية العمل',
          workingEnd: 'نهاية العمل',
          department: 'القسم',
          selectDepartment: 'اختر القسم'
        },
        actions: {
          save: 'حفظ الطبيب',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      addDepartment: {
        title: 'إضافة قسم',
        failed: 'فشل إنشاء قسم',
        fields: {
          name: 'الاسم',
          description: 'الوصف'
        },
        actions: {
          save: 'حفظ القسم',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      addAppointment: {
        title: 'إضافة موعد',
        failed: 'فشل إنشاء موعد',
        fields: {
          patient: 'المريض',
          selectPatient: 'اختر مريض',
          doctor: 'الطبيب',
          selectDoctor: 'اختر طبيب',
          date: 'التاريخ',
          time: 'الوقت',
          reason: 'السبب',
          notes: 'ملاحظات',
          room: 'الغرفة (اختياري)',
          selectRoom: 'اختر غرفة (اختياري)'
        },
        actions: {
          save: 'حفظ الموعد',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      addBill: {
        title: 'إضافة فاتورة',
        failed: 'فشل إنشاء فاتورة',
        fields: {
          patient: 'المريض',
          selectPatient: 'اختر مريض',
          billDate: 'تاريخ الفاتورة',
          dueDate: 'تاريخ الاستحقاق',
          insuranceProvider: 'شركة التأمين',
          insuranceNumber: 'رقم التأمين',
          insuranceCoverage: 'نسبة التغطية التأمينية'
        },
        items: {
          title: 'البنود',
          description: 'الوصف',
          qty: 'الكمية',
          unitPrice: 'سعر الوحدة',
          addItem: 'إضافة بند',
          remove: 'إزالة'
        },
        actions: {
          save: 'حفظ الفاتورة',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      addMedicine: {
        title: 'إضافة دواء',
        failed: 'فشل إنشاء دواء',
        fields: {
          name: 'الاسم',
          genericName: 'الاسم العام',
          dosageForm: 'شكل الجرعة',
          strength: 'القوة',
          manufacturer: 'المنتج',
          price: 'السعر',
          stockQuantity: 'كمية المخزون',
          minimumStock: 'الحد الأدنى',
          unit: 'الوحدة',
          expiryDate: 'تاريخ الانتهاء',
          batchNumber: 'رقم الدفعة',
          requiresPrescription: 'يحتاج وصفة طبية',
          yes: 'نعم',
          no: 'لا'
        },
        actions: {
          save: 'حفظ الدواء',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      addRoom: {
        title: 'إضافة غرفة',
        failed: 'فشل إنشاء غرفة',
        fields: {
          roomNumber: 'رقم الغرفة',
          roomType: 'نوع الغرفة',
          floor: 'الطابق',
          building: 'المبنى',
          description: 'الوصف',
          capacity: 'السعة',
          hourlyRate: 'السعر بالساعة',
          departmentId: 'رقم القسم',
          isAvailable: 'متاحة'
        },
        actions: {
          save: 'حفظ الغرفة',
          saving: 'جارٍ الحفظ…',
          cancel: 'إلغاء'
        }
      },
      chatbot: {
        title: 'مساعد المستشفى',
        subtitle: 'اسألني عن أي شيء يتعلق بخدماتنا',
        welcome: 'مرحباً! أنا مساعد المستشفى. يمكنني مساعدتك في:\n- حجز المواعيد\n- البحث عن الأطباء\n- معلومات الأقسام\n- استفسارات الفواتير\n- توفر الأدوية\n- خدمات الطوارئ\n- ساعات العمل وموقع المستشفى\n\nكيف يمكنني مساعدتك اليوم؟',
        placeholder: 'اكتب رسالتك هنا...',
        send: 'إرسال',
        sending: 'جارٍ الإرسال...',
        error: 'عذراً، حدث خطأ. يرجى المحاولة مرة أخرى أو الاتصال بفريق الدعم.',
        noResponse: 'لم أتلق رداً. يرجى المحاولة مرة أخرى.'
      },
      nursingPage: {
        badge: 'رعاية تمريضية موثوقة',
        title: 'إدارة التمريض المتكاملة',
        subtitle: 'تغطية دائمة بجانب السرير وفي المنزل مع مراقبة ذكية وتثقيف ودعم للعائلة.',
        cta: {
          primary: 'احجز استشارة تمريضية',
          secondary: 'استعرض الأقسام'
        },
        metrics: {
          coverage: { label: 'التغطية', value: '24/7' },
          response: { label: 'الاستجابة السريعة', value: '< 4 دقائق' },
          satisfaction: { label: 'رضا العائلات', value: '97%' }
        },
        promise: {
          label: 'وعد الرعاية',
          title: 'رعاية إنسانية مدعومة بالتقنية',
          desc: 'قادة الورديات ينسقون مع الأطباء والصيادلة وأخصائيي العلاج الطبيعي لضمان حصول كل مريض على مسار رعاية شخصي.'
        },
        pillars: {
          compassion: {
            title: 'تعاطف',
            desc: 'حضور إنساني، استماع فعّال وحس ثقافي.'
          },
          coverage: {
            title: 'تغطية 24/7',
            desc: 'فرق دوارة تغطي العناية المركزة والطوارئ والأجنحة والخدمة المنزلية.'
          },
          expertise: {
            title: 'خبرة سريرية',
            desc: 'بروتوكولات مبنية على الأدلة وعلاج وريدي ورعاية متقدمة للجروح.'
          },
          familySupport: {
            title: 'دعم العائلة',
            desc: 'تحديثات واضحة وجلسات توعية وتوجيه عند الخروج.'
          }
        },
        programs: {
          label: 'برامجنا',
          title: 'برامج تمريض متخصصة',
          desc: 'كل برنامج يضم ممرضين معتمدين وتوثيقاً رقمياً وسلالم تصعيد واضحة.',
          emergency: {
            title: 'جاهزية الطوارئ',
            desc: 'فرق فرز سريع بشهادات إصابات وإنعاش متقدم.'
          },
          icu: {
            title: 'دقة العناية المركزة',
            desc: 'إدارة أجهزة التنفس ومراقبة مكافحة العدوى.'
          },
          pediatrics: {
            title: 'راحة الأطفال',
            desc: 'غرف متمركزة حول العائلة وأدوات تشتيت الألم ومتابعة النمو.'
          },
          homeCare: {
            title: 'انتقال الرعاية المنزلية',
            desc: 'متابعة تمريضية عن بُعد، مطابقة الأدوية وتنسيق التأهيل.'
          }
        },
        support: {
          title: 'القيادة وقنوات الدعم',
          desc: 'مدربو التمريض والجودة ومديرو الحالات على تواصل مع كل وردية.',
          rounds: { label: 'جولات القيادة اليومية', value: '3+' },
          consultations: { label: 'استشارات بجانب السرير', value: '18 يومياً' },
          training: { label: 'ورش تطوير', value: '12 شهرياً' },
          ctaTitle: 'دعنا نصمم رحلة رعايتك',
          ctaDesc: 'شاركنا تفاصيل حالتك لنقترح المزيج التمريضي الأنسب.',
          ctaButton: 'تحدث مع قائدة التمريض'
        },
        dataset: {
          label: 'قاعدة بيانات التمريض',
          title: 'بيانات التغطية الميدانية والافتراضية',
          desc: 'استكشف التوزيع البشري، أوقات التغطية ومجالات التركيز لكل وحدة تمريض. يمكنك تنزيل الملف الخام للتحليل والتقارير.',
          searchPlaceholder: 'ابحث باسم الوحدة أو الموقع أو قائدة التمريض…',
          download: 'تنزيل ملف CSV',
          columns: {
            unit: 'الوحدة',
            wing: 'الموقع / الجناح',
            lead: 'قائدة التمريض',
            nurses: 'عدد الممرضات',
            coverage: 'التغطية',
            focus: 'مجالات التركيز'
          },
          emptyState: 'لا توجد وحدات مطابقة لـ "{{query}}". جرّب كلمة أخرى.'
        }
      }
    }
  }
};

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: 'en',
    supportedLngs: ['en', 'ar'],
    interpolation: { escapeValue: false },
    detection: {
      order: ['querystring', 'localStorage', 'navigator'],
      lookupQuerystring: 'lng',
      caches: ['localStorage']
    }
  });

const applyDirection = (lng) => {
  const isRtl = lng === 'ar';
  if (typeof document !== 'undefined') {
    document.documentElement.setAttribute('lang', lng);
    document.documentElement.setAttribute('dir', isRtl ? 'rtl' : 'ltr');
  }
};

applyDirection(i18n.resolvedLanguage || i18n.language);

i18n.on('languageChanged', applyDirection);

export default i18n;


