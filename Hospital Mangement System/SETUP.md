# دليل إعداد وتشغيل نظام إدارة المستشفى

## المتطلبات الأساسية

### 1. تثبيت .NET 8.0 SDK
- تحميل وتثبيت .NET 8.0 SDK من [Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
- التحقق من التثبيت: `dotnet --version`

### 2. تثبيت SQL Server
- تثبيت SQL Server 2019 أو أحدث
- أو استخدام SQL Server Express (مجاني)
- أو استخدام SQL Server LocalDB (مضمن مع Visual Studio)

### 3. تثبيت Visual Studio (اختياري)
- Visual Studio 2022 Community (مجاني)
- أو Visual Studio Code مع C# extension

## خطوات الإعداد

### 1. استنساخ المشروع
```bash
git clone <repository-url>
cd "Hospital Mangement System"
```

### 2. تحديث Connection String
قم بتحديث connection string في `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HospitalManagementSystem;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**ملاحظة:** استبدل `Server=.` بـ `Server=localhost` أو اسم الخادم الخاص بك.

### 3. تثبيت الحزم
```bash
cd "Hospital Mangement System"
dotnet restore
```

### 4. إنشاء قاعدة البيانات
```bash
dotnet ef database create
```

### 5. تشغيل المشروع
```bash
dotnet run
```

## الوصول للتطبيق

- **API Base URL:** `https://localhost:7102` أو `http://localhost:5230`
- **Swagger UI:** `https://localhost:7102/swagger` أو `http://localhost:5230/swagger`

## البيانات الافتراضية

عند التشغيل الأول، سيتم إنشاء:

### المستخدم الافتراضي (Admin)
- **Email:** admin@hospital.com
- **Password:** Admin@123
- **Role:** Admin

### الأقسام الافتراضية
- Cardiology (أمراض القلب)
- Neurology (الأعصاب)
- Orthopedics (العظام)
- Pediatrics (طب الأطفال)
- Emergency (الطوارئ)

### الغرف الافتراضية
- C101 - Cardiology Consultation
- N201 - Neurology Consultation
- O101 - Orthopedics Consultation
- P201 - Pediatrics Consultation
- E001 - Emergency Room

### الأدوية الافتراضية
- Paracetamol (باراسيتامول)
- Amoxicillin (أموكسيسيلين)
- Ibuprofen (إيبوبروفين)

## اختبار API

### 1. تسجيل الدخول
```bash
POST https://localhost:7102/api/auth/login
Content-Type: application/json

{
  "email": "admin@hospital.com",
  "password": "Admin@123"
}
```

### 2. استخدام Token
أضف الـ token المستلم في header:
```
Authorization: Bearer <your-token>
```

### 3. اختبار Endpoints
- `GET /api/patients` - قائمة المرضى
- `GET /api/doctors` - قائمة الأطباء
- `GET /api/appointments` - قائمة المواعيد
- `GET /api/dashboard/stats` - إحصائيات لوحة التحكم

## استكشاف الأخطاء

### مشاكل قاعدة البيانات
1. تأكد من تشغيل SQL Server
2. تحقق من صحة connection string
3. تأكد من وجود صلاحيات إنشاء قاعدة البيانات

### مشاكل Port
إذا كان الـ port مشغول:
1. غير الـ port في `launchSettings.json`
2. أو أوقف التطبيق الذي يستخدم نفس الـ port

### مشاكل Authentication
1. تأكد من استخدام HTTPS للـ JWT
2. تحقق من صحة الـ JWT settings في `appsettings.json`

## التطوير

### إضافة ميزات جديدة
1. إنشاء Model جديد في مجلد `Models`
2. إضافة DbSet في `HospitalDbContext`
3. إنشاء DTOs في مجلد `DTOs`
4. إنشاء Service في مجلد `Services`
5. إنشاء Controller في مجلد `Controllers`
6. إضافة AutoMapper mapping في `AutoMapperProfile`

### إضافة Migration جديد
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## النشر

### 1. إعداد Production
```bash
dotnet publish -c Release -o ./publish
```

### 2. تحديث appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=HospitalManagementSystem;User Id=your-user;Password=your-password;"
  },
  "JwtSettings": {
    "SecretKey": "YourProductionSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "HospitalManagementSystem",
    "Audience": "HospitalManagementSystem",
    "ExpiryInMinutes": 60
  }
}
```

## الدعم

للحصول على المساعدة:
1. تحقق من ملف README.md
2. راجع وثائق Swagger
3. افتح issue في المستودع

## الأمان

⚠️ **تحذير:** تأكد من تغيير الـ JWT Secret Key في بيئة الإنتاج!

```json
{
  "JwtSettings": {
    "SecretKey": "YourVerySecureSecretKeyForProductionEnvironment!"
  }
}
```
