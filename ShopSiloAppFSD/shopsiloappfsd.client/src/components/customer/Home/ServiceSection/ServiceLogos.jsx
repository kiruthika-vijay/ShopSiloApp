import React from 'react';
import { FaTruck, FaHeadset, FaMoneyBillWave } from 'react-icons/fa'; // React Icons

const ServiceLogos = () => {
    return (
        <div className="flex flex-col items-center my-5 mt-20 mb-20">
            <div className="flex space-x-10">
                {/* Free and Fast Delivery */}
                <div className="flex flex-col items-center">
                    <div className="bg-gray-300 p-3 rounded-full flex items-center justify-center">
                        <div className="bg-black p-3 rounded-full">
                            <FaTruck className="text-white text-xl" />
                        </div>
                    </div>
                    <span className="text-center mt-2 font-semibold">Free and Fast Delivery</span>
                    <span className="text-sm text-center">Free delivery for all orders over Rs. 1500.</span>
                </div>

                {/* 24/7 Customer Service */}
                <div className="flex flex-col items-center">
                    <div className="bg-gray-300 p-3 rounded-full flex items-center justify-center">
                        <div className="bg-black p-3 rounded-full">
                            <FaHeadset className="text-white text-xl" />
                        </div>
                    </div>
                    <span className="text-center mt-2 font-semibold">24/7 Customer Service</span>
                    <span className="text-sm text-center">Friendly 24/7 customer support.</span>
                </div>

                {/* Money Back Guarantee */}
                <div className="flex flex-col items-center">
                    <div className="bg-gray-300 p-3 rounded-full flex items-center justify-center">
                        <div className="bg-black p-3 rounded-full">
                            <FaMoneyBillWave className="text-white text-xl" />
                        </div>
                    </div>
                    <span className="text-center mt-2 font-semibold">Money Back Guarantee</span>
                    <span className="text-sm text-center">We return money within 30 days.</span>
                </div>
            </div>
        </div>
    );
};

export default ServiceLogos;
