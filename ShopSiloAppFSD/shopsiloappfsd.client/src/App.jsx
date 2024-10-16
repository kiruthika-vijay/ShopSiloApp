import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { Outlet } from 'react-router-dom'; // Import Outlet
import Login from './components/customer/Auth/Login';
import Register from './components/customer/Auth/Register';
import ForgotPassword from './components/customer/Auth/ForgotPassword';
import ResetPassword from './components/customer/Auth/ResetPassword';
import NotFound from './components/customer/AppLayout/NotFound';
import HomePage from './components/customer/Home/HomePage';
import Wishlist from './components/customer/Home/WishList/Wishlist';
import Logout from './components/customer/Auth/Logout';
import ShoppingCart from './components/customer/ShoppingCart/ShoppingCart';
import AboutPage from './components/customer/Home/NavBarPages/About';
import ContactPage from './components/customer/Home/NavBarPages/Contact';
import ApplyDiscount from './components/customer/ShoppingCart/Discount/DiscountComponent';
import ProductDescriptionPage from './components/customer/Product/ProductDescriptionPage';
import AllProducts from './components/customer/Product/Product';
import CategoryProducts from './components/customer/Home/CategoryNavBar/CategoryProducts';
import SearchResults from './components/customer/Product/SearchResults';
import Shop from './components/common/Footer/FooterNavPages/Shop';
import PrivacyPolicy from './components/common/Footer/FooterNavPages/PrivacyPolicy';
import TermsAndConditions from './components/common/Footer/FooterNavPages/TermsAndConditions';
import FAQ from './components/common/Footer/FooterNavPages/FAQ';
import Suggestions from './components/customer/Home/WishList/SuggestionProducts';
import ProfilePage from './components/customer/Home/Profile/ProfilePage';
import OrderSummary from './components/customer/Order/OrderSummary';
import Header from './components/common/Header/Header';
import CustomerFooter from './components/common/Footer/Footer';
import SellerLayout from './components/seller/SellerLayout/SellerLayout';
import SellerDashboard from './components/seller/Home/Dashboard/SellerDashboard';
import ProductList from './components/seller/Home/Product/ProductList';
import ProductForm from './components/seller/Home/Product/ProductForm';
import Profile from './components/seller/Home/Profile/Profile';
import SellerLogin from './components/seller/Auth/SellerLogin';
import SellerRegister from './components/seller/Auth/SellerRegister';
import { CountProvider } from './components/common/Header/CountContext.jsx';
import InventoryManagement from './components/seller/Home/Product/InventoryManagement';
import AdminLayout from './components/Admin/AdminLayout/AdminLayout';
import CustomerList from './components/Admin/Customerdetails/CustomerList';
import SellerList from './components/Admin/SellerDetails/SellerList';
import AuditReportList from './components/AuditLog/AuditReportList';
import AdminDashboard from './components/Admin/AdminDashboard/AdminDashboard';
import AdminProductList from './components/Admin/ProductDetails/AdminProductList';
import EditProductForm from './components/seller/Home/Product/EditProductForm';
import OrderView from './components/seller/Home/Order/OrderView';
import AdminLogin from './components/Admin/Auth/AdminLogin';
import CategoryBrowseProducts from './components/customer/Home/Categories/CategoryBrowseProducts';

const CustomerLayout = () => (
    <CountProvider>
        <Header />
        <main className="main">
            <Outlet />  {/* This renders the child routes, now below the Header */}
        </main>
        <CustomerFooter />
    </CountProvider>
);

const App = () => {
    return (
        <Routes>
            {/* 404 Not Found Route */}
            <Route path="*" element={<NotFound />} />
            <Route path="reset-password/:token" element={<ResetPassword />} />
            <Route path="logout" element={<Logout />} />

            {/* Customer Portal Routes */}

            <Route path="/customer/login" element={<Login />} />
            <Route path="/customer/register" element={<Register />} />
            <Route path="/customer" element={<CustomerLayout />}>
                {/* Default route for /customer */}
                <Route index element={<HomePage />} />  
                <Route path="home" element={<HomePage />} />
                <Route path="product/:productId" element={<ProductDescriptionPage />} />
                <Route path="categories/:categoryId" element={<CategoryProducts />} />
                <Route path="browse-categories/:categoryId" element={<CategoryBrowseProducts />} />
                <Route path="search" element={<SearchResults />} />
                <Route path="products" element={<AllProducts />} />
                <Route path="suggestions" element={<Suggestions />} />
                <Route path="order-summary" element={<OrderSummary />} />
                <Route path="shop" element={<Shop />} />
                <Route path="privacy" element={<PrivacyPolicy />} />
                <Route path="terms" element={<TermsAndConditions />} />
                <Route path="faq" element={<FAQ />} />
                <Route path="about" element={<AboutPage />} />
                <Route path="discount" element={<ApplyDiscount />} />
                <Route path="contact" element={<ContactPage />} />
                <Route path="profile" element={<ProfilePage />} />
                <Route path="wishlist" element={<Wishlist />} />
                <Route path="cart" element={<ShoppingCart />} />
                <Route path="forgot-password" element={<ForgotPassword />} />
                <Route path="account/profile" element={<ProfilePage />} />
            </Route>

            {/* Seller Portal Routes */}

            <Route path="/seller/login" index element={<SellerLogin />} />
            <Route path="/seller/register" element={<SellerRegister />} />

            <Route path="/seller" element={<SellerLayout />}>
                <Route path="dashboard" element={<SellerDashboard />} />
                <Route path="editproduct/:productId" element={<EditProductForm />} /> {/* Add the edit product route */}
                <Route path="products/productlist" element={<ProductList />} />
                <Route path="products/productupload" element={<ProductForm />} />
                <Route path="products/inventoryManagement" element={<InventoryManagement />} />
                <Route path="account/profile" element={<Profile />} />
                <Route path="orders/orderview" element={<OrderView/> }/>
            </Route>

            {/* Admin Portal Routes */}

            <Route path="/admin/login" index element={<AdminLogin />} />

            <Route path="/admin" element={<AdminLayout />} >
                <Route path="customers" element={<CustomerList />} />
                <Route path="products" element={<AdminProductList />} />
                <Route path="sellers" element={<SellerList />} />
                <Route path="autitlogs" element={<AuditReportList />} />
                <Route path="dashboard" index={true} element={<AdminDashboard />} />
            </Route>

        </Routes>
    );
};

export default App;