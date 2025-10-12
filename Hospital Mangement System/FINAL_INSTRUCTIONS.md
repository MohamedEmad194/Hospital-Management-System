# 🏥 تعليمات التشغيل النهائية - نظام إدارة المستشفى

## ✅ المشروع جاهز ومحدث!

تم إصلاح جميع المشاكل وتحديث المشروع. الآن يمكنك تشغيله بسهولة.

## 🚀 طرق التشغيل

### الطريقة الأولى: التشغيل السريع (الأسهل)
```bash
# انقر مرتين على الملف
START_HOSPITAL_SYSTEM.bat
```

### الطريقة الثانية: PowerShell
```powershell
# انقر بزر الماوس الأيمن واختر "Run with PowerShell"
START_HOSPITAL_SYSTEM.ps1
```

### الطريقة الثالثة: Command Line
```bash
cd "Hospital Mangement System"
dotnet run
```

## 🔧 المتطلبات

### 1. .NET 8.0 SDK
- تحميل من: https://dotnet.microsoft.com/download/dotnet/8.0
- التحقق: `dotnet --version`

### 2. SQL Server
- SQL Server 2019 أو أحدث
- أو SQL Server Express
- أو SQL Server LocalDB

## 🌐 الوصول للتطبيق

بعد التشغيل، افتح المتصفح على:

- **الرئيسي:** `https://localhost:7102`
- **Swagger UI:** `https://localhost:7102/swagger`
- **HTTP:** `http://localhost:5230`

## 🔑 بيانات تسجيل الدخول

- **Email:** admin@hospital.com
- **Password:** Admin@123

## 📊 ما سيحدث تلقائياً

1. ✅ **إنشاء قاعدة البيانات** `HospitalManagementSystem`
2. ✅ **إنشاء 15+ جدول** مع العلاقات
3. ✅ **إضافة البيانات الأولية:**
   - مستخدم إداري
   - 5 أقسام (قلب، أعصاب، عظام، أطفال، طوارئ)
   - 5 غرف استشارة
   - 3 أدوية أساسية
   - أدوار المستخدمين

## 🎯 الميزات المتاحة

### إدارة المرضى
- تسجيل المرضى الجدد
- تحديث المعلومات
- البحث والفلترة
- إدارة التاريخ الطبي

### إدارة الأطباء
- تسجيل الأطباء
- إدارة التخصصات
- جداول العمل
- رسوم الاستشارة

### إدارة المواعيد
- حجز المواعيد
- إدارة الحالات
- البحث المتقدم
- إدارة الغرف

### السجلات الطبية
- تسجيل التشخيصات
- إدارة الوصفات
- تتبع الفحوصات
- العلامات الحيوية

### إدارة الفواتير
- إنشاء الفواتير
- معالجة المدفوعات
- تتبع المتأخرات
- إدارة التأمين

### إدارة الأدوية
- إدارة المخزون
- تتبع انتهاء الصلاحية
- تنبيهات المخزون المنخفض
- الوصفات الطبية

### لوحة التحكم
- إحصائيات شاملة
- تقارير المبيعات
- إحصائيات المواعيد
- إحصائيات الأقسام

## 🔍 اختبار API

### 1. تسجيل الدخول
```http
POST https://localhost:7102/api/auth/login
Content-Type: application/json

{
  "email": "admin@hospital.com",
  "password": "Admin@123"
}
```

### 2. استخدام Token
```http
GET https://localhost:7102/api/patients
Authorization: Bearer <your-token>
```

### 3. إنشاء مريض جديد
```http
POST https://localhost:7102/api/patients
Authorization: Bearer <your-token>
Content-Type: application/json

{
  "firstName": "أحمد",
  "lastName": "محمد",
  "nationalId": "1234567890",
  "email": "ahmed@example.com",
  "phoneNumber": "01234567890",
  "dateOfBirth": "1990-01-01",
  "gender": "Male"
}
```

## 🛠️ استكشاف الأخطاء

### مشكلة: "لا أجد قاعدة البيانات"
**الحل:**
1. تأكد من تشغيل SQL Server
2. استخدم `START_HOSPITAL_SYSTEM.bat` للتشغيل التلقائي

### مشكلة: "Server not found"
**الحل:**
1. افتح Services (services.msc)
2. ابحث عن SQL Server (MSSQLSERVER)
3. تأكد من أنه Running

### مشكلة: "Build failed"
**الحل:**
```bash
dotnet clean
dotnet restore
dotnet build
```

## 📁 ملفات المشروع المهمة

- `START_HOSPITAL_SYSTEM.bat` - تشغيل سريع
- `START_HOSPITAL_SYSTEM.ps1` - تشغيل PowerShell
- `README.md` - دليل المشروع
- `DATABASE_SETUP.md` - دليل قاعدة البيانات
- `API_DOCUMENTATION.md` - وثائق API
- `QUICK_START.md` - دليل البدء السريع

## 🎉 النجاح!

إذا رأيت هذه الرسالة في المتصفح:
```
Welcome to Swagger UI
Hospital Management System API v1
```

فقد نجح التشغيل! 🎊

## 📞 الدعم

إذا واجهت أي مشاكل:
1. راجع `DATABASE_SETUP.md`
2. راجع `QUICK_START.md`
3. تحقق من أن SQL Server يعمل
4. تأكد من تثبيت .NET 8.0

---

**🏥 نظام إدارة المستشفى جاهز للاستخدام! استمتع بالتطوير! 🚀**
