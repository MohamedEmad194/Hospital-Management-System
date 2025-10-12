# دليل إعداد قاعدة البيانات - نظام إدارة المستشفى

## المشكلة الشائعة: "لا أجد قاعدة البيانات في SQL Server"

هذا دليل شامل لحل مشكلة إنشاء قاعدة البيانات في SQL Server.

## الحلول المتاحة

### الحل الأول: التشغيل التلقائي (الأسهل) ✅

1. **تأكد من تشغيل SQL Server**
   - افتح **Services** (services.msc)
   - ابحث عن **SQL Server (MSSQLSERVER)**
   - تأكد من أنه **Running**

2. **شغل المشروع**
   ```bash
   # Windows
   run_updated.bat
   
   # PowerShell
   .\run_updated.ps1
   ```

3. **سيتم إنشاء قاعدة البيانات تلقائياً** مع جميع الجداول والبيانات

### الحل الثاني: إنشاء قاعدة البيانات يدوياً

#### 1. إنشاء قاعدة البيانات في SSMS

1. افتح **SQL Server Management Studio (SSMS)**
2. اتصل بخادم SQL Server
3. انقر بزر الماوس الأيمن على **Databases**
4. اختر **New Database**
5. اسم قاعدة البيانات: `HospitalManagementSystem`
6. انقر **OK**

#### 2. تحديث Connection String (إذا لزم الأمر)

في `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HospitalManagementSystem;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### الحل الثالث: استخدام Entity Framework Migrations

#### 1. في Visual Studio

1. افتح **Package Manager Console**
2. اكتب الأوامر التالية:

```powershell
# تثبيت EF Tools
Install-Package Microsoft.EntityFrameworkCore.Tools

# إنشاء Migration
Add-Migration InitialCreate

# تطبيق Migration
Update-Database
```

#### 2. في Command Line

```bash
cd "Hospital Mangement System\Hospital Mangement System"

# تثبيت EF Tools
dotnet tool install --global dotnet-ef

# إنشاء Migration
dotnet ef migrations add InitialCreate

# إنشاء قاعدة البيانات
dotnet ef database update
```

## استكشاف الأخطاء

### خطأ: "Login failed"

**الحل:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HospitalManagementSystem;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

### خطأ: "Server not found"

**الحل:**
1. تأكد من تشغيل SQL Server
2. جرب connection strings مختلفة:

```json
// للخادم المحلي
"Server=.;Database=HospitalManagementSystem;Trusted_Connection=true;TrustServerCertificate=true;"

// لـ SQL Server Express
"Server=.\\SQLEXPRESS;Database=HospitalManagementSystem;Trusted_Connection=true;TrustServerCertificate=true;"

// للخادم المحدد
"Server=localhost;Database=HospitalManagementSystem;Trusted_Connection=true;TrustServerCertificate=true;"
```

### خطأ: "Database does not exist"

**الحل:**
1. أنشئ قاعدة البيانات يدوياً في SSMS
2. أو استخدم `EnsureCreated()` في الكود (موجود بالفعل)

## التحقق من النجاح

### 1. في SQL Server Management Studio

بعد التشغيل الناجح، يجب أن ترى:

**قاعدة البيانات:**
- `HospitalManagementSystem`

**الجداول:**
- `AspNetUsers` (المستخدمين)
- `AspNetRoles` (الأدوار)
- `AspNetUserRoles` (صلاحيات المستخدمين)
- `Patients` (المرضى)
- `Doctors` (الأطباء)
- `Departments` (الأقسام)
- `Appointments` (المواعيد)
- `MedicalRecords` (السجلات الطبية)
- `Prescriptions` (الوصفات الطبية)
- `Medicines` (الأدوية)
- `Bills` (الفواتير)
- `Rooms` (الغرف)
- `Schedules` (الجداول)
- `Staff` (الموظفين)

### 2. في التطبيق

1. افتح المتصفح على: `https://localhost:7102`
2. يجب أن ترى Swagger UI
3. جرب تسجيل الدخول:
   - Email: `admin@hospital.com`
   - Password: `Admin@123`

## البيانات الافتراضية

عند التشغيل الأول، سيتم إنشاء:

### المستخدم الإداري
- **Email:** admin@hospital.com
- **Password:** Admin@123
- **Role:** Admin

### الأقسام
- Cardiology (أمراض القلب)
- Neurology (الأعصاب)
- Orthopedics (العظام)
- Pediatrics (طب الأطفال)
- Emergency (الطوارئ)

### الغرف
- C101 - Cardiology Consultation
- N201 - Neurology Consultation
- O101 - Orthopedics Consultation
- P201 - Pediatrics Consultation
- E001 - Emergency Room

### الأدوية
- Paracetamol (باراسيتامول)
- Amoxicillin (أموكسيسيلين)
- Ibuprofen (إيبوبروفين)

## نصائح مهمة

1. **تأكد من تشغيل SQL Server** قبل تشغيل التطبيق
2. **استخدم Windows Authentication** إذا أمكن
3. **تحقق من Firewall** إذا كان لديك مشاكل في الاتصال
4. **استخدم TrustServerCertificate=true** لتجنب مشاكل الشهادات

## إذا استمرت المشكلة

1. **تحقق من إصدار SQL Server** (يجب أن يكون 2019 أو أحدث)
2. **جرب SQL Server Express** إذا لم يكن لديك النسخة الكاملة
3. **تحقق من صلاحيات المستخدم** في Windows
4. **راجع سجلات الأخطاء** في Visual Studio Output

## الدعم

إذا واجهت مشاكل أخرى:
1. راجع ملف `README.md`
2. تحقق من `SETUP.md`
3. افتح issue في المستودع

---

**ملاحظة:** المشروع مصمم لإنشاء قاعدة البيانات تلقائياً عند التشغيل الأول. فقط تأكد من تشغيل SQL Server!
