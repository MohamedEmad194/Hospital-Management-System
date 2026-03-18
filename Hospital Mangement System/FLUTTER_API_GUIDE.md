# دليل API لتطبيق Flutter | Flutter API Integration Guide

## 📋 جدول المحتويات | Table of Contents

- [معلومات أساسية | Base Information](#معلومات-أساسية--base-information)
- [المصادقة | Authentication](#المصادقة--authentication)
- [نقاط النهاية | API Endpoints](#نقاط-النهاية--api-endpoints)
- [أمثلة Flutter | Flutter Examples](#أمثلة-flutter--flutter-examples)
- [Postman Collection](#postman-collection)

---

## معلومات أساسية | Base Information

### Base URL
```
Development: http://localhost:5230
أو
Development: https://localhost:7102

Production: (يتم تحديثها لاحقاً)
```

### Content-Type
جميع الطلبات يجب أن تحتوي على:
```
Content-Type: application/json
```

### Response Format
جميع الردود تكون بصيغة JSON

---

## المصادقة | Authentication

الـ API يستخدم **JWT (JSON Web Token)** للمصادقة.

### كيفية الحصول على Token

بعد تسجيل الدخول أو التسجيل، ستحصل على token في الاستجابة:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-16T10:30:00Z",
  "user": {
    "id": "1",
    "email": "user@example.com",
    "roles": ["Patient"]
  }
}
```

### استخدام Token في الطلبات

يجب إرسال الـ token في Header:

```
Authorization: Bearer <your-jwt-token>
```

### حفظ Token في Flutter

احفظ الـ token في SharedPreferences أو secure storage:

```dart
// بعد تسجيل الدخول
await sharedPreferences.setString('auth_token', token);
await sharedPreferences.setString('user_id', userId);
```

---

## نقاط النهاية | API Endpoints

### 1️⃣ المصادقة | Authentication

#### تسجيل الدخول | Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@hospital.com",
  "password": "Admin@123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-16T10:30:00Z",
  "user": {
    "id": "1",
    "email": "admin@hospital.com",
    "firstName": "Admin",
    "lastName": "User",
    "roles": ["Admin"]
  }
}
```

#### التسجيل | Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "nationalId": "1234567890",
  "dateOfBirth": "1990-01-01",
  "gender": "Male",
  "address": "123 Main St",
  "phoneNumber": "1234567890"
}
```

#### تغيير كلمة المرور | Change Password
```http
POST /api/auth/change-password
Authorization: Bearer <token>
Content-Type: application/json

{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "confirmNewPassword": "NewPassword123!"
}
```

---

### 2️⃣ المرضى | Patients

#### الحصول على جميع المرضى
```http
GET /api/patients
Authorization: Bearer <token>
```

**Required Roles:** Admin, Doctor, Staff

#### الحصول على مريض محدد
```http
GET /api/patients/{id}
Authorization: Bearer <token>
```

#### إنشاء مريض جديد
```http
POST /api/patients
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "nationalId": "9876543210",
  "email": "jane.smith@example.com",
  "phoneNumber": "0987654321",
  "dateOfBirth": "1985-05-15",
  "gender": "Female",
  "address": "456 Oak Ave",
  "emergencyContactName": "John Smith",
  "emergencyContactPhone": "1111111111",
  "insuranceProvider": "Health Insurance Co",
  "insuranceNumber": "INS123456"
}
```

#### تحديث مريض
```http
PUT /api/patients/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Jane Updated",
  "phoneNumber": "0987654321",
  "address": "789 Pine St"
}
```

#### حذف مريض
```http
DELETE /api/patients/{id}
Authorization: Bearer <token>
```

**Required Roles:** Admin

---

### 3️⃣ الأطباء | Doctors

#### الحصول على جميع الأطباء
```http
GET /api/doctors
Authorization: Bearer <token>
```

#### الحصول على طبيب محدد
```http
GET /api/doctors/{id}
Authorization: Bearer <token>
```

#### إنشاء طبيب جديد
```http
POST /api/doctors
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Dr. Ahmed",
  "lastName": "Hassan",
  "nationalId": "1111111111",
  "email": "ahmed.hassan@hospital.com",
  "phoneNumber": "2222222222",
  "dateOfBirth": "1980-03-20",
  "gender": "Male",
  "address": "Hospital Address",
  "licenseNumber": "LIC123456",
  "specialization": "Cardiology",
  "yearsOfExperience": 15,
  "consultationFee": 200.00,
  "workingHoursStart": "09:00:00",
  "workingHoursEnd": "17:00:00",
  "departmentId": 1
}
```

**Required Roles:** Admin

---

### 4️⃣ المواعيد | Appointments

#### الحصول على جميع المواعيد
```http
GET /api/appointments
Authorization: Bearer <token>
```

**Note:** النتائج تعتمد على دور المستخدم:
- **Admin/Staff:** جميع المواعيد
- **Doctor:** مواعيد الطبيب فقط
- **Patient:** مواعيد المريض فقط

#### الحصول على موعد محدد
```http
GET /api/appointments/{id}
Authorization: Bearer <token>
```

#### الحصول على مواعيد مريض
```http
GET /api/appointments/patient/{patientId}
Authorization: Bearer <token>
```

#### الحصول على مواعيد طبيب
```http
GET /api/appointments/doctor/{doctorId}
Authorization: Bearer <token>
```

#### الحصول على المواعيد المتاحة لطبيب في تاريخ محدد
```http
GET /api/appointments/available-slots/{doctorId}/{date}
Authorization: Bearer <token>
```

**Example:**
```
GET /api/appointments/available-slots/1/2024-01-15
```

#### إنشاء موعد جديد
```http
POST /api/appointments
Authorization: Bearer <token>
Content-Type: application/json

{
  "appointmentDate": "2024-01-15",
  "appointmentTime": "10:00:00",
  "reason": "Regular checkup",
  "notes": "Patient has been feeling tired",
  "isFollowUp": false,
  "patientId": 1,
  "doctorId": 1,
  "roomId": 1
}
```

#### تحديث موعد
```http
PUT /api/appointments/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "appointmentDate": "2024-01-16",
  "appointmentTime": "11:00:00",
  "reason": "Follow-up appointment"
}
```

#### إلغاء موعد
```http
PUT /api/appointments/{id}/cancel
Authorization: Bearer <token>
```

#### إكمال موعد
```http
PUT /api/appointments/{id}/complete
Authorization: Bearer <token>
Content-Type: application/json

{
  "diagnosis": "General fatigue, recommend rest",
  "treatment": "Get adequate sleep, regular exercise"
}
```

**Required Roles:** Admin, Doctor, Staff

---

### 5️⃣ الوصفات الطبية | Prescriptions

#### الحصول على جميع الوصفات
```http
GET /api/prescriptions
Authorization: Bearer <token>
```

#### الحصول على وصفة محددة
```http
GET /api/prescriptions/{id}
Authorization: Bearer <token>
```

#### الحصول على وصفات مريض
```http
GET /api/prescriptions/patient/{patientId}
Authorization: Bearer <token>
```

#### إنشاء وصفة جديدة
```http
POST /api/prescriptions
Authorization: Bearer <token>
Content-Type: application/json

{
  "prescriptionDate": "2024-01-15",
  "validUntil": "2024-01-22",
  "instructions": "Take with food",
  "notes": "Complete the full course",
  "patientId": 1,
  "doctorId": 1,
  "prescriptionItems": [
    {
      "medicineName": "Paracetamol",
      "dosage": "500mg",
      "frequency": "Twice daily",
      "duration": "7 days",
      "instructions": "Take with food",
      "quantity": 14,
      "unitPrice": 5.00,
      "medicineId": 1
    }
  ]
}
```

**Required Roles:** Admin, Doctor, Staff

#### صرف وصفة
```http
PUT /api/prescriptions/{id}/dispense
Authorization: Bearer <token>
```

**Required Roles:** Admin, Staff

---

### 6️⃣ السجلات الطبية | Medical Records

#### الحصول على سجلات مريض
```http
GET /api/medicalrecords/patient/{patientId}
Authorization: Bearer <token>
```

#### إنشاء سجل طبي جديد
```http
POST /api/medicalrecords
Authorization: Bearer <token>
Content-Type: application/json

{
  "recordDate": "2024-01-15",
  "recordType": "Consultation",
  "symptoms": "Fatigue, headache",
  "diagnosis": "Stress-related symptoms",
  "treatment": "Rest and relaxation",
  "prescription": "Paracetamol 500mg",
  "notes": "Patient advised to reduce stress",
  "bloodPressure": "120/80",
  "temperature": "98.6°F",
  "heartRate": "72",
  "weight": "70kg",
  "height": "170cm",
  "patientId": 1,
  "doctorId": 1
}
```

**Required Roles:** Admin, Doctor, Staff

---

### 7️⃣ الفواتير | Bills

#### الحصول على فواتير مريض
```http
GET /api/bills/patient/{patientId}
Authorization: Bearer <token>
```

#### الحصول على جميع الفواتير
```http
GET /api/bills
Authorization: Bearer <token>
```

**Required Roles:** Admin, Staff

#### إنشاء فاتورة جديدة
```http
POST /api/bills
Authorization: Bearer <token>
Content-Type: application/json

{
  "billDate": "2024-01-15",
  "dueDate": "2024-01-22",
  "notes": "Consultation and medication",
  "insuranceProvider": "Health Insurance Co",
  "insuranceNumber": "INS123456",
  "insuranceCoverage": 80.00,
  "patientId": 1,
  "billItems": [
    {
      "description": "Consultation Fee",
      "category": "Consultation",
      "quantity": 1,
      "unitPrice": 200.00,
      "notes": "Cardiology consultation"
    },
    {
      "description": "Paracetamol 500mg",
      "category": "Medicine",
      "quantity": 14,
      "unitPrice": 5.00,
      "notes": "Pain relief medication"
    }
  ]
}
```

#### معالجة دفعة
```http
POST /api/bills/{id}/payment
Authorization: Bearer <token>
Content-Type: application/json

{
  "amount": 150.00,
  "paymentMethod": "Credit Card",
  "notes": "Payment processed successfully"
}
```

---

### 8️⃣ الأدوية | Medicines

#### الحصول على جميع الأدوية
```http
GET /api/medicines
Authorization: Bearer <token>
```

#### الحصول على دواء محدد
```http
GET /api/medicines/{id}
Authorization: Bearer <token>
```

#### إنشاء دواء جديد
```http
POST /api/medicines
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Paracetamol",
  "genericName": "Acetaminophen",
  "manufacturer": "Pharma Co",
  "dosageForm": "Tablet",
  "strength": "500mg",
  "stockQuantity": 1000,
  "unitPrice": 5.00,
  "expiryDate": "2025-12-31",
  "category": "Analgesic"
}
```

**Required Roles:** Admin, Staff

---

### 9️⃣ الأقسام | Departments

#### الحصول على جميع الأقسام
```http
GET /api/departments
Authorization: Bearer <token>
```

#### الحصول على قسم محدد
```http
GET /api/departments/{id}
Authorization: Bearer <token>
```

---

### 🔟 الغرف | Rooms

#### الحصول على جميع الغرف
```http
GET /api/rooms
Authorization: Bearer <token>
```

#### الحصول على غرفة محددة
```http
GET /api/rooms/{id}
Authorization: Bearer <token>
```

---

### 1️⃣1️⃣ لوحة التحكم | Dashboard

#### إحصائيات لوحة التحكم
```http
GET /api/dashboard/stats
Authorization: Bearer <token>
```

**Response:**
```json
{
  "totalPatients": 150,
  "totalDoctors": 25,
  "totalAppointments": 500,
  "todayAppointments": 15,
  "pendingBills": 30,
  "totalRevenue": 50000.00
}
```

#### المواعيد الأخيرة
```http
GET /api/dashboard/recent-appointments?count=10
Authorization: Bearer <token>
```

#### إحصائيات الإيرادات
```http
GET /api/dashboard/revenue
Authorization: Bearer <token>
```

**Required Roles:** Admin, Staff

---

### 1️⃣2️⃣ Chatbot

#### إرسال رسالة للـ Chatbot
```http
POST /api/chatbot/message
Content-Type: application/json

{
  "message": "What are the visiting hours?"
}
```

**Note:** هذا الـ endpoint لا يحتاج authentication

---

## أمثلة Flutter | Flutter Examples

### مثال: خدمة API في Flutter

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class ApiService {
  final String baseUrl = 'http://localhost:5230';
  String? _token;

  Future<void> setToken(String token) {
    _token = token;
  }

  Map<String, String> get _headers => {
    'Content-Type': 'application/json',
    if (_token != null) 'Authorization': 'Bearer $_token',
  };

  // تسجيل الدخول
  Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/auth/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'email': email,
        'password': password,
      }),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      _token = data['token'];
      return data;
    } else {
      throw Exception('Login failed: ${response.body}');
    }
  }

  // الحصول على جميع المرضى
  Future<List<dynamic>> getPatients() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/patients'),
      headers: _headers,
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception('Failed to load patients: ${response.body}');
    }
  }

  // إنشاء موعد
  Future<Map<String, dynamic>> createAppointment(Map<String, dynamic> appointmentData) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/appointments'),
      headers: _headers,
      body: jsonEncode(appointmentData),
    );

    if (response.statusCode == 201 || response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception('Failed to create appointment: ${response.body}');
    }
  }

  // الحصول على مواعيد مريض
  Future<List<dynamic>> getPatientAppointments(int patientId) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/appointments/patient/$patientId'),
      headers: _headers,
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception('Failed to load appointments: ${response.body}');
    }
  }
}
```

### مثال: استخدام الخدمة

```dart
class MyApp extends StatefulWidget {
  @override
  _MyAppState createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  final ApiService _apiService = ApiService();
  List<dynamic> _patients = [];

  @override
  void initState() {
    super.initState();
    _loginAndLoadData();
  }

  Future<void> _loginAndLoadData() async {
    try {
      // تسجيل الدخول
      final loginResponse = await _apiService.login(
        'admin@hospital.com',
        'Admin@123',
      );
      
      print('Login successful: ${loginResponse['user']['email']}');
      
      // تحميل المرضى
      final patients = await _apiService.getPatients();
      setState(() {
        _patients = patients;
      });
    } catch (e) {
      print('Error: $e');
    }
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(title: Text('HMS Flutter App')),
        body: ListView.builder(
          itemCount: _patients.length,
          itemBuilder: (context, index) {
            final patient = _patients[index];
            return ListTile(
              title: Text('${patient['firstName']} ${patient['lastName']}'),
              subtitle: Text(patient['email']),
            );
          },
        ),
      ),
    );
  }
}
```

---

## Postman Collection

يمكنك استيراد ملف Postman الموجود في:
```
Postman/HMS_Flutter.postman_collection.json
```

### كيفية الاستخدام:
1. افتح Postman
2. اضغط على Import
3. اختر ملف `HMS_Flutter.postman_collection.json`
4. قم بتعيين المتغيرات:
   - `baseUrl`: `http://localhost:5230`
   - `token`: (سيتم تعيينه تلقائياً بعد تسجيل الدخول)

---

## رموز الحالة | Status Codes

| Code | المعنى | Description |
|------|--------|-------------|
| 200 | نجاح | OK - Request successful |
| 201 | تم الإنشاء | Created - Resource created successfully |
| 400 | طلب غير صحيح | Bad Request - Invalid request data |
| 401 | غير مصرح | Unauthorized - Authentication required |
| 403 | محظور | Forbidden - Insufficient permissions |
| 404 | غير موجود | Not Found - Resource not found |
| 500 | خطأ في الخادم | Internal Server Error - Server error |

---

## معالجة الأخطاء | Error Handling

### مثال على استجابة خطأ:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "The Email field is required."
    ]
  }
}
```

### معالجة الأخطاء في Flutter:
```dart
try {
  final response = await _apiService.login(email, password);
  // Handle success
} on http.ClientException catch (e) {
  // Network error
  print('Network error: $e');
} catch (e) {
  // Other errors
  final errorData = jsonDecode(e.toString());
  print('Error: ${errorData['message']}');
}
```

---

## ملاحظات مهمة | Important Notes

1. **HTTPS في Production:** يجب استخدام HTTPS في الإنتاج
2. **Token Expiration:** الـ token له مدة صلاحية، احفظ تاريخ الانتهاء وتحقق منه قبل كل طلب
3. **Refresh Token:** في حالة انتهاء الـ token، قم بتسجيل الدخول مرة أخرى
4. **Permissions:** تحقق من صلاحيات المستخدم قبل إرسال الطلبات
5. **Error Handling:** قم بمعالجة جميع الأخطاء بشكل صحيح

---

## اختبار API | Testing API

### اختبار سريع باستخدام curl:

```bash
# تسجيل الدخول
curl -X POST http://localhost:5230/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@hospital.com","password":"Admin@123"}'

# الحصول على المرضى (استبدل TOKEN بالـ token الفعلي)
curl -X GET http://localhost:5230/api/patients \
  -H "Authorization: Bearer TOKEN"
```

---

## الدعم | Support

إذا واجهت أي مشاكل:
1. تحقق من Base URL
2. تأكد من صحة الـ Token
3. تحقق من صلاحيات المستخدم (Roles)
4. راجع رسائل الخطأ في الـ Response

---

**تاريخ آخر تحديث:** 2024-01-15
