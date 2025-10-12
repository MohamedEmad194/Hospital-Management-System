# دليل البدء السريع - نظام إدارة المستشفى

## 🚀 التشغيل في 3 خطوات

### الخطوة 1: تأكد من تشغيل SQL Server
1. افتح **Services** (اضغط Win+R واكتب `services.msc`)
2. ابحث عن **SQL Server (MSSQLSERVER)**
3. تأكد من أنه **Running** (إذا لم يكن، انقر عليه واختر Start)

### الخطوة 2: شغل المشروع
```bash
# Windows
run_updated.bat

# أو PowerShell
.\run_updated.ps1
```

### الخطوة 3: افتح التطبيق
- **URL:** `https://localhost:7102`
- **Swagger UI:** `https://localhost:7102/swagger`

## 🔑 بيانات تسجيل الدخول الافتراضية

- **Email:** admin@hospital.com
- **Password:** Admin@123

## ✅ ما سيحدث تلقائياً

1. **إنشاء قاعدة البيانات** `HospitalManagementSystem`
2. **إنشاء جميع الجداول** (15+ جدول)
3. **إضافة البيانات الأولية** (أقسام، غرف، أدوية، مستخدم إداري)
4. **تشغيل API** مع وثائق Swagger

## 🔧 إذا واجهت مشاكل

### مشكلة: "لا أجد قاعدة البيانات"
**الحل:** اتبع دليل `DATABASE_SETUP.md`

### مشكلة: "Server not found"
**الحل:** 
1. تأكد من تشغيل SQL Server
2. جرب connection string مختلف في `appsettings.json`

### مشكلة: "Build failed"
**الحل:**
```bash
dotnet restore
dotnet clean
dotnet build
```

## 📱 اختبار API

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

## 🎯 الميزات المتاحة

- ✅ إدارة المرضى
- ✅ إدارة الأطباء
- ✅ حجز المواعيد
- ✅ السجلات الطبية
- ✅ الوصفات الطبية
- ✅ إدارة الفواتير
- ✅ إدارة الأدوية
- ✅ إدارة الأقسام والغرف
- ✅ لوحة تحكم مع إحصائيات

## 📊 الإحصائيات

- **API Endpoints:** 80+
- **Database Tables:** 15+
- **Services:** 10+
- **Controllers:** 12+

## 🆘 الدعم

إذا واجهت أي مشاكل:
1. راجع `DATABASE_SETUP.md` لمشاكل قاعدة البيانات
2. راجع `README.md` للدليل الكامل
3. راجع `API_DOCUMENTATION.md` لاستخدام API

---

**🎉 مبروك! نظام إدارة المستشفى جاهز للاستخدام!**
