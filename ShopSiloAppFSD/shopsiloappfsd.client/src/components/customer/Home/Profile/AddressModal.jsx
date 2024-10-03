import React, { useState, useEffect } from 'react';
import { apiClient } from '../../../common/Axios/auth';
import Select from 'react-select';  // react-select for searchable dropdowns
import Toggle from 'react-toggle';  // For the toggle switches
import 'react-toggle/style.css';    // For toggle styling

const AddressModal = ({ onClose, title, addressData, customerID }) => {
    // State to hold form data
    const [formData, setFormData] = useState({
        addressLine1: '',
        addressLine2: '',
        city: '',
        state: '',
        postalCode: '',
        country: 'India',  // Default to India
        isBillingAddress: true,
        isShippingAddress: true,
        customerID: customerID || 0,  // If customerID is provided as a prop
    });

    // Country options (India as default)
    const countryOptions = [
        { value: 'India', label: 'India' }
    ];

    // Indian states options
    const stateOptions = [
        { value: 'Andhra Pradesh', label: 'Andhra Pradesh' },
        { value: 'Arunachal Pradesh', label: 'Arunachal Pradesh' },
        { value: 'Assam', label: 'Assam' },
        { value: 'Bihar', label: 'Bihar' },
        { value: 'Chhattisgarh', label: 'Chhattisgarh' },
        { value: 'Goa', label: 'Goa' },
        { value: 'Gujarat', label: 'Gujarat' },
        { value: 'Haryana', label: 'Haryana' },
        { value: 'Himachal Pradesh', label: 'Himachal Pradesh' },
        { value: 'Jharkhand', label: 'Jharkhand' },
        { value: 'Karnataka', label: 'Karnataka' },
        { value: 'Kerala', label: 'Kerala' },
        { value: 'Madhya Pradesh', label: 'Madhya Pradesh' },
        { value: 'Maharashtra', label: 'Maharashtra' },
        { value: 'Manipur', label: 'Manipur' },
        { value: 'Meghalaya', label: 'Meghalaya' },
        { value: 'Mizoram', label: 'Mizoram' },
        { value: 'Nagaland', label: 'Nagaland' },
        { value: 'Odisha', label: 'Odisha' },
        { value: 'Punjab', label: 'Punjab' },
        { value: 'Rajasthan', label: 'Rajasthan' },
        { value: 'Sikkim', label: 'Sikkim' },
        { value: 'Tamil Nadu', label: 'Tamil Nadu' },
        { value: 'Telangana', label: 'Telangana' },
        { value: 'Tripura', label: 'Tripura' },
        { value: 'Uttar Pradesh', label: 'Uttar Pradesh' },
        { value: 'Uttarakhand', label: 'Uttarakhand' },
        { value: 'West Bengal', label: 'West Bengal' }
    ];

    // Cities in India (examples for popular cities)
    const cityOptions = [
        { value: 'Mumbai', label: 'Mumbai' },
        { value: 'Delhi', label: 'Delhi' },
        { value: 'Bengaluru', label: 'Bengaluru' },
        { value: 'Hyderabad', label: 'Hyderabad' },
        { value: 'Ahmedabad', label: 'Ahmedabad' },
        { value: 'Chennai', label: 'Chennai' },
        { value: 'Kolkata', label: 'Kolkata' },
        { value: 'Pune', label: 'Pune' },
        { value: 'Jaipur', label: 'Jaipur' },
        { value: 'Surat', label: 'Surat' },
        { value: 'Lucknow', label: 'Lucknow' },
        { value: 'Kanpur', label: 'Kanpur' },
        { value: 'Nagpur', label: 'Nagpur' },
        { value: 'Visakhapatnam', label: 'Visakhapatnam' },
        { value: 'Bhopal', label: 'Bhopal' },
        { value: 'Patna', label: 'Patna' },
        { value: 'Vadodara', label: 'Vadodara' },
        { value: 'Ghaziabad', label: 'Ghaziabad' },
        { value: 'Ludhiana', label: 'Ludhiana' },
        { value: 'Agra', label: 'Agra' },
        { value: 'Nashik', label: 'Nashik' }
    ];

    // Initialize the form data when the modal opens
    useEffect(() => {
        if (addressData) {
            setFormData({ ...addressData, customerID: customerID || addressData.customerID });
        }
    }, [addressData, customerID]);

    // Handle input changes
    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData({
            ...formData,
            [name]: type === 'checkbox' ? checked : value
        });
    };

    // Handle dropdown changes (react-select)
    const handleSelectChange = (selectedOption, fieldName) => {
        setFormData({
            ...formData,
            [fieldName]: selectedOption ? selectedOption.value : ''
        });
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            // Assuming you have an API endpoint to save or update the address
            const response = await apiClient.post(`/ShippingAddress`, formData); // Ensure the correct endpoint
            console.log("Address saved:", response.data); // Optional: log the response
            onClose(); // Close the modal after saving
        } catch (error) {
            console.error("Error saving address:", error);
            // Optionally handle error response (e.g., show error message)
        }
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center z-50">
            <div className="bg-black opacity-50 absolute inset-0" onClick={onClose}></div>
            <div className="bg-white rounded-lg p-6 relative z-10 w-96 overflow-y-auto max-h-[90vh]">
                <h2 className="text-lg font-semibold mb-4">{title}</h2>
                <form onSubmit={handleSubmit}>
                    {/* Address Line 1 */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Address Line 1</label>
                        <input
                            type="text"
                            name="addressLine1"
                            value={formData.addressLine1 || ''}
                            onChange={handleChange}
                            className="border rounded w-full p-2"
                            required
                        />
                    </div>

                    {/* Address Line 2 */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Address Line 2</label>
                        <input
                            type="text"
                            name="addressLine2"
                            value={formData.addressLine2 || ''}
                            onChange={handleChange}
                            className="border rounded w-full p-2"
                        />
                    </div>

                    {/* Country Dropdown */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Country</label>
                        <Select
                            name="country"
                            value={countryOptions.find(option => option.value === formData.country)}
                            onChange={(selectedOption) => handleSelectChange(selectedOption, 'country')}
                            options={countryOptions}
                            placeholder="Select Country"
                        />
                    </div>

                    {/* State Dropdown */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">State</label>
                        <Select
                            name="state"
                            value={stateOptions.find(option => option.value === formData.state)}
                            onChange={(selectedOption) => handleSelectChange(selectedOption, 'state')}
                            options={stateOptions}
                            placeholder="Select State"
                        />
                    </div>

                    {/* City Dropdown */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">City</label>
                        <Select
                            name="city"
                            value={cityOptions.find(option => option.value === formData.city)}
                            onChange={(selectedOption) => handleSelectChange(selectedOption, 'city')}
                            options={cityOptions}
                            placeholder="Select City"
                        />
                    </div>

                    {/* Postal Code */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Postal Code</label>
                        <input
                            type="text"
                            name="postalCode"
                            value={formData.postalCode || ''}
                            onChange={handleChange}
                            className="border rounded w-full p-2"
                            required
                        />
                    </div>

                    {/* Toggle for Billing Address */}
                    <div className="flex items-center mb-4">
                        <Toggle
                            checked={formData.isBillingAddress}
                            onChange={handleChange}
                            name="isBillingAddress"
                        />
                        <label className="ml-2 text-sm">Is Billing Address?</label>
                    </div>

                    {/* Toggle for Shipping Address */}
                    <div className="flex items-center mb-4">
                        <Toggle
                            checked={formData.isShippingAddress}
                            onChange={handleChange}
                            name="isShippingAddress"
                        />
                        <label className="ml-2 text-sm">Is Shipping Address?</label>
                    </div>

                    <button type="submit" className="bg-blue-500 text-white rounded p-2 hover:bg-blue-600">
                        Save Address
                    </button>
                </form>
            </div>
        </div>
    );
};

export default AddressModal;
