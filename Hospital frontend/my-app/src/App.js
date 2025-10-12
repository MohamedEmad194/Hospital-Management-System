import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import PatientsList from './pages/PatientsList';
import DoctorsList from './pages/DoctorsList';
import AppointmentsList from './pages/AppointmentsList';
import BillsList from './pages/BillsList';
import DepartmentsList from './pages/DepartmentsList';
import MedicinesList from './pages/MedicinesList';
import AddDoctor from './pages/AddDoctor';
import AddMedicine from './pages/AddMedicine';
import AddPatient from './pages/AddPatient';
import AddDepartment from './pages/AddDepartment';
import AddAppointment from './pages/AddAppointment';
import AddBill from './pages/AddBill';

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route element={<ProtectedRoute />}> 
            <Route element={<Layout />}> 
              <Route path="/" element={<Dashboard />} />
              <Route path="/patients" element={<PatientsList />} />
              <Route path="/patients/add" element={<AddPatient />} />
              <Route path="/doctors" element={<DoctorsList />} />
              <Route path="/doctors/add" element={<AddDoctor />} />
              <Route path="/appointments" element={<AppointmentsList />} />
              <Route path="/appointments/add" element={<AddAppointment />} />
              <Route path="/bills" element={<BillsList />} />
              <Route path="/bills/add" element={<AddBill />} />
              <Route path="/departments" element={<DepartmentsList />} />
              <Route path="/departments/add" element={<AddDepartment />} />
              <Route path="/medicines" element={<MedicinesList />} />
              <Route path="/medicines/add" element={<AddMedicine />} />
            </Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
