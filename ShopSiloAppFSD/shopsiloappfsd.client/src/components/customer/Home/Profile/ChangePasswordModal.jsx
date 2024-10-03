import { useState } from "react";
import { apiClient, getToken } from "../../../common/Axios/auth";
import { useNavigate } from 'react-router-dom'; // Import useNavigate instead

const ChangePasswordModal = ({ onClose, oldPassword }) => {
    const [formData, setFormData] = useState({ oldPassword: '', newPassword: '', confirmPassword: '' });
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate(); // Hook to programmatically navigate

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
        setErrorMessage(''); // Clear any previous error messages
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Validate the old password
        if (oldPassword !== formData.oldPassword) {
            setErrorMessage("Old Password didn't match. Please enter the correct one.");
            return;
        }

        // Validate new password
        if (formData.newPassword !== formData.confirmPassword) {
            setErrorMessage('Passwords do not match!');
            return;
        }

        if (formData.newPassword === oldPassword) {
            setErrorMessage('New password cannot be the same as old password.');
            return;
        }

        try {
            var token = getToken();
            // Send a POST request with the form data
            await apiClient.post(`/Users/profile/change-password?newPassword=${formData.newPassword}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            // On success, navigate to the profile page
            navigate('/customer/profile'); // Adjust the path as necessary
            onClose(); // Close the modal
        } catch (error) {
            console.error('Error changing password:', error);
            if (error.response && error.response.data.errors) {
                // Set the error message if there's a validation error from the backend
                setErrorMessage(error.response.data.errors.newPassword[0]);
            } else {
                setErrorMessage('An error occurred while changing the password. Please try again.');
            }
        }
    };

    return (
        <div className="modal">
            <div className="modal-content bg-white p-6 rounded-lg shadow-lg">
                <h2 className="text-xl font-bold mb-4">Change Password</h2>
                {errorMessage && <p className="text-red-500 mb-4">{errorMessage}</p>}
                <form onSubmit={handleSubmit}>
                    <input
                        type="password"
                        name="oldPassword"
                        placeholder="Old Password"
                        onChange={handleChange}
                        className="border rounded p-2 mb-4 w-full"
                        required
                    />
                    <input
                        type="password"
                        name="newPassword"
                        value={formData.newPassword}
                        placeholder="New Password"
                        onChange={handleChange}
                        className="border rounded p-2 mb-4 w-full"
                        required
                    />
                    <input
                        type="password"
                        name="confirmPassword"
                        placeholder="Confirm Password"
                        onChange={handleChange}
                        className="border rounded p-2 mb-4 w-full"
                        required
                    />
                    <button type="submit" className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600">
                        Change Password
                    </button>
                    <button type="button" onClick={onClose} className="ml-4 py-2 px-4 border border-gray-300 rounded hover:bg-gray-200">
                        Cancel
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ChangePasswordModal;
