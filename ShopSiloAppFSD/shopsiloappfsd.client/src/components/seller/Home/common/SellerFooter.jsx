import React from 'react';

const SellerFooter = ({ sidebarCollapsed }) => {
    return (
        <div
            className={`bg-[#C1BCAC] py-2 ${sidebarCollapsed ? 'ml-16' : 'ml-60'} mt-auto`} // Background, padding, margin-left, margin-top
        >
            <div className="container mx-auto text-center">
                <p className="text-[#214e34] text-base font-medium"> {/* Text color, font size, and weight */}
                    © 2024 Seller Portal. All Rights Reserved.
                </p>
            </div>
        </div>
    );
};

export default SellerFooter;
