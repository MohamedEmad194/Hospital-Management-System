# Hospital Management System

نظام إدارة مستشفى شامل مبني باستخدام ASP.NET Core 8.0 و SQL Server.

## المميزات

### إدارة المرضى
- تسجيل المرضى الجدد
- تحديث معلومات المرضى
- البحث عن المرضى
- إدارة التاريخ الطبي
- إدارة الحساسيات والأدوية الحالية

### إدارة الأطباء
- تسجيل الأطباء الجدد
- إدارة التخصصات والشهادات
- إدارة جداول العمل
- إدارة رسوم الاستشارة

### إدارة المواعيد
- حجز المواعيد
- إدارة حالة المواعيد
- البحث والفلترة
- إدارة الغرف والمواعيد المتاحة

### إدارة السجلات الطبية
- تسجيل التشخيصات
- إدارة الوصفات الطبية
- تتبع الفحوصات والنتائج
- إدارة العلامات الحيوية

### إدارة الفواتير
- إنشاء الفواتير
- إدارة المدفوعات
- تتبع الفواتير المتأخرة
- إدارة التأمين

### إدارة الأدوية
- إدارة المخزون
- تتبع انتهاء الصلاحية
- تنبيهات المخزون المنخفض
- إدارة الوصفات الطبية

### إدارة الأقسام والغرف
- إدارة أقسام المستشفى
- إدارة الغرف والمواعيد
- تخصيص الغرف للأقسام

### لوحة التحكم
- إحصائيات شاملة
- تقارير المبيعات
- إحصائيات المواعيد
- إحصائيات الأقسام

## التقنيات المستخدمة

- **ASP.NET Core 8.0** - إطار العمل الأساسي
- **Entity Framework Core** - ORM لإدارة قاعدة البيانات
- **SQL Server** - قاعدة البيانات
- **AutoMapper** - تحويل البيانات بين الطبقات
- **JWT Authentication** - نظام المصادقة
- **Swagger/OpenAPI** - وثائق API
- **Serilog** - نظام السجلات
- **FluentValidation** - التحقق من صحة البيانات

## متطلبات النظام

- .NET 8.0 SDK
- SQL Server 2019 أو أحدث
- Visual Studio 2022 أو VS Code

## التثبيت والتشغيل

### 1. استنساخ المشروع
```bash
git clone <repository-url>
cd "Hospital Mangement System"
```

### 2. تحديث قاعدة البيانات
```bash
cd "Hospital Mangement System"
dotnet ef database update
```

### 3. تشغيل المشروع
```bash
dotnet run
```

### 4. الوصول للتطبيق
- API: `https://localhost:7000`
- Swagger UI: `https://localhost:7000/swagger`

## إعداد قاعدة البيانات

### Connection String
تأكد من تحديث connection string في `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HospitalManagementSystem;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### إنشاء قاعدة البيانات
```bash
dotnet ef database create
```

## API Endpoints

### المصادقة
- `POST /api/auth/register` - تسجيل مستخدم جديد
- `POST /api/auth/login` - تسجيل الدخول
- `POST /api/auth/change-password` - تغيير كلمة المرور
- `GET /api/auth/profile` - الحصول على الملف الشخصي

### المرضى
- `GET /api/patients` - الحصول على جميع المرضى
- `GET /api/patients/{id}` - الحصول على مريض محدد
- `POST /api/patients` - إضافة مريض جديد
- `PUT /api/patients/{id}` - تحديث مريض
- `DELETE /api/patients/{id}` - حذف مريض

### الأطباء
- `GET /api/doctors` - الحصول على جميع الأطباء
- `GET /api/doctors/{id}` - الحصول على طبيب محدد
- `POST /api/doctors` - إضافة طبيب جديد
- `PUT /api/doctors/{id}` - تحديث طبيب
- `DELETE /api/doctors/{id}` - حذف طبيب

### المواعيد
- `GET /api/appointments` - الحصول على جميع المواعيد
- `GET /api/appointments/{id}` - الحصول على موعد محدد
- `POST /api/appointments` - حجز موعد جديد
- `PUT /api/appointments/{id}` - تحديث موعد
- `PUT /api/appointments/{id}/cancel` - إلغاء موعد
- `PUT /api/appointments/{id}/complete` - إكمال موعد

### السجلات الطبية
- `GET /api/medicalrecords` - الحصول على جميع السجلات
- `GET /api/medicalrecords/{id}` - الحصول على سجل محدد
- `POST /api/medicalrecords` - إضافة سجل جديد
- `PUT /api/medicalrecords/{id}` - تحديث سجل

### الفواتير
- `GET /api/bills` - الحصول على جميع الفواتير
- `GET /api/bills/{id}` - الحصول على فاتورة محددة
- `POST /api/bills` - إنشاء فاتورة جديدة
- `POST /api/bills/{id}/payment` - معالجة دفعة

### الأدوية
- `GET /api/medicines` - الحصول على جميع الأدوية
- `GET /api/medicines/{id}` - الحصول على دواء محدد
- `POST /api/medicines` - إضافة دواء جديد
- `PUT /api/medicines/{id}` - تحديث دواء

### الأقسام
- `GET /api/departments` - الحصول على جميع الأقسام
- `GET /api/departments/{id}` - الحصول على قسم محدد
- `POST /api/departments` - إضافة قسم جديد
- `PUT /api/departments/{id}` - تحديث قسم

### الغرف
- `GET /api/rooms` - الحصول على جميع الغرف
- `GET /api/rooms/{id}` - الحصول على غرفة محددة
- `POST /api/rooms` - إضافة غرفة جديدة
- `PUT /api/rooms/{id}` - تحديث غرفة

### الجداول
- `GET /api/schedules` - الحصول على جميع الجداول
- `GET /api/schedules/{id}` - الحصول على جدول محدد
- `POST /api/schedules` - إضافة جدول جديد
- `PUT /api/schedules/{id}` - تحديث جدول

### الوصفات الطبية
- `GET /api/prescriptions` - الحصول على جميع الوصفات
- `GET /api/prescriptions/{id}` - الحصول على وصفة محددة
- `POST /api/prescriptions` - إضافة وصفة جديدة
- `PUT /api/prescriptions/{id}/dispense` - صرف وصفة

### لوحة التحكم
- `GET /api/dashboard/stats` - إحصائيات عامة
- `GET /api/dashboard/recent-appointments` - المواعيد الأخيرة
- `GET /api/dashboard/revenue` - إحصائيات الإيرادات

## الأمان

- مصادقة JWT
- تشفير كلمات المرور
- صلاحيات المستخدمين
- CORS مفعل

## السجلات

يستخدم النظام Serilog لتسجيل الأحداث والأخطاء في:
- Console
- ملفات السجلات في مجلد `logs/`

## المساهمة

1. Fork المشروع
2. إنشاء فرع للميزة الجديدة
3. Commit التغييرات
4. Push للفرع
5. إنشاء Pull Request

## الترخيص

هذا المشروع مرخص تحت رخصة MIT.

## الدعم

للحصول على الدعم، يرجى فتح issue في المستودع.
