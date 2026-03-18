# حل مشكلة تسجيل الدخول - 401 Unauthorized

## المشكلة:
حصول على خطأ `401 Unauthorized` مع رسالة "Invalid email or password"

## الحلول:

### الحل السريع (الموصى به):

#### 1. إنشاء حسابات User للأطباء والمرضى والموظفين:

افتح المتصفح أو Postman واذهب إلى:
```
POST http://localhost:5230/api/TestCredentials/ensure-users
```

سيعمل هذا على إنشاء جميع الحسابات المطلوبة.

#### 2. الحصول على بيانات تسجيل الدخول:

```
GET http://localhost:5230/api/TestCredentials/all
```

ستحصل على قائمة كاملة بجميع الحسابات المتاحة.

### بيانات تسجيل الدخول الافتراضية:

#### Admin:
```
Email: admin@hospital.com
Password: Admin@123
Role: Admin
```

#### Doctor (أمثلة):
```
Email: ahmed.hassan@hospital.com
Password: Doctor@123
Role: Doctor

Email: sarah.mohamed@hospital.com
Password: Doctor@123
Role: Doctor

Email: omar.ali@hospital.com
Password: Doctor@123
Role: Doctor
```

#### Patient:
- Password لجميع المرضى: `Patient@123`
- Role: `Patient`
- Email: (أي email من قائمة المرضى - احصل عليها من GET /api/TestCredentials/all)

#### Staff/Nurse:
- Password لجميع الموظفين: `Staff@123`
- Role: `Nurse`
- Email: (أي email من قائمة الموظفين)

### كلمات المرور الموحدة:

- **Admin**: `Admin@123`
- **Doctor**: `Doctor@123`
- **Patient**: `Patient@123`
- **Staff/Nurse**: `Staff@123`

### خطوات التحقق:

1. تأكد أن الـ Backend يعمل على `http://localhost:5230`
2. افتح Swagger: `http://localhost:5230/swagger`
3. جرب `POST /api/TestCredentials/ensure-users`
4. ثم جرب `GET /api/TestCredentials/all` لرؤية جميع الحسابات
5. استخدم أي email من القائمة مع كلمة المرور المناسبة للدور

### ملاحظات:

- النظام الآن يحاول إنشاء الحساب تلقائياً إذا كان Entity موجود لكن User غير موجود
- إذا فشل تسجيل الدخول، تأكد من:
  - البريد الإلكتروني صحيح
  - كلمة المرور صحيحة
  - الدور المختار صحيح (Admin, Doctor, Patient, أو Nurse)
  - الحساب موجود (استخدم ensure-users إذا لم يكن موجوداً)

