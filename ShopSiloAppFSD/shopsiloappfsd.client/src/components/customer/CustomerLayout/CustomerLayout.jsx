//import React from 'react';
//import Header from '../../common/Header/Header';
//import CustomerFooter from '../../common/Footer/Footer';
//import { CountProvider } from '../../common/Header/CountContext';
//import { Outlet } from 'react-router-dom'; // Import Outlet

//const CustomerLayout = ({ children }) => {
//    return (
//        <div>
//            <CountProvider>
//                <Header/>
//                <div style={{ display: 'flex' }}>
//                    <div className="main d-flex">
//                        <div className="sidebarWrapper">
//                            <SellerSidebar />
//                        </div>
//                    </div>
//                    <main
//                        style={{
//                            flex: 1,
//                            padding: '20px',
//                            paddingTop: '20px',
//                            marginLeft: sidebarCollapsed ? '70px' : '240px', // Adjust main content based on sidebar state
//                            transition: 'margin-left 0.4s ease' // Smooth transition effect
//                        }}
//                    >
//                        <Outlet />  {/* This renders the child routes */}
//                    </main>
//                </div>
//                <CustomerFooter/>
//                   {/*<SellerFooter sidebarCollapsed={sidebarCollapsed} />*/}
//            </CountProvider>
//        </div>
//    );
//};

//export default CustomerLayout;
