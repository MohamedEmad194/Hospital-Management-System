# 🔧 حل مشكلة تسجيل الدخول 401

## الخطوات السريعة:

### 1. إنشاء حسابات المستخدمين (إذا لم تكن موجودة)

افتح Postman أو متصفح واتصل بـ:
```
POST http://localhost:5230/api/TestCredentials/ensure-users
```
سيقوم هذا بإنشاء حسابات لجميع الأطباء والمرضى والموظفين الموجودين في قاعدة البيانات.

### 2. الحصول على بيانات تسجيل الدخول

```
GET http://localhost:5230/api/TestCredentials/all
```
أو للحصول على بيانات سريعة:
```
GET http://localhost:5230/api/QuickTest/quick-credentials
```

### 3. كلمات المرور الافتراضية:

- **Admin**: `Admin@123`
- **Doctor**: `Doctor@123`
- **Patient**: `Patient@123`
- **Staff/Nurse**: `Staff@123`

### 4. تأكد من:

✅ البريد الإلكتروني صحيح
✅ كلمة المرور صحيحة (مع الحروف الكبيرة والصغيرة والأرقام)
✅ اختيار الدور الصحيح (Admin, Doctor, Patient, Nurse)
✅ المستخدم موجود في قاعدة البيانات

### 5. المشاكل الشائعة:

#### المشكلة: "User not found"
**الحل**: اتصل بـ `POST /api/TestCredentials/ensure-users`

#### المشكلة: "Invalid password"
**الحل**: استخدم كلمة المرور الافتراضية الصحيحة حسب الدور

#### المشكلة: "User does not have the [Role] role"
**الحل**: تأكد من اختيار الدور الصحيح. استخدم `GET /api/TestCredentials/all` لمعرفة الأدوار المتاحة

#### المشكلة: "No doctor/patient/staff record found"
**الحل**: تأكد من أن السجل موجود في قاعدة البيانات (Doctors/Patients/Staff tables)

## ✨ الميزة الجديدة:

الآن، عند تسجيل الدخول، إذا كان البريد الإلكتروني موجوداً في جدول Doctor/Patient/Staff ولكن ليس لديه حساب User، سيتم إنشاء الحساب تلقائياً! 🎉

## اختبار سريع:

```bash
# اختبار تسجيل الدخول
POST http://localhost:5230/api/QuickTest/test-login
Content-Type: application/json

{
  "email": "admin@hospital.com",
  "password": "Admin@123",
  "role": "Admin"
}
```

