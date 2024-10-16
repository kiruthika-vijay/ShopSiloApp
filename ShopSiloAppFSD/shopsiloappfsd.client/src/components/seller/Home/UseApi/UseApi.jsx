import { useCallback } from 'react';
import { apiClient } from '../../../common/Axios/auth';

const useApi = () => {
    // Fetch the seller profile by sellerID
    const fetchProfile = useCallback(async (sellerID) => {
        if (!sellerID) {
            throw new Error("Seller ID is required to fetch profile.");
        }
        try {
            const response = await apiClient.get(`/Seller/${sellerID}`);
            return response.data; // Return fetched profile data
        } catch (error) {
            console.error('Error fetching profile:', error);
            throw error; // Rethrow error to handle in the component
        }
    }, []);

    // Update the seller profile
    const updateProfile = useCallback(async (profileData) => {
        if (!profileData.sellerID) {
            throw new Error("Seller ID is required to update profile.");
        }
        try {
            await apiClient.put(`/Seller/${profileData.sellerID}`, profileData); // Use sellerID for update
        } catch (error) {
            console.error('Error updating profile:', error);
            throw error; // Rethrow error if needed
        }
    }, []);

    // Add a new seller profile
    const addProfile = async (profileData) => {
        try {
            const response = await apiClient.post('/Seller', profileData);
            return response.data; // Optionally return the newly added profile
        } catch (error) {
            console.error('Error adding profile:', error);
            throw error; // Rethrow error if needed
        }
    };

    return { fetchProfile, updateProfile, addProfile };
};

export default useApi;