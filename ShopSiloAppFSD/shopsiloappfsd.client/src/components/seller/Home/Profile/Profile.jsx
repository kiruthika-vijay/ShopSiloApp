import React, { useState, useEffect } from 'react';
import { Typography, Paper, Button, TextField } from '@mui/material';
import useApi from '../UseApi/UseApi';
import { getUserId } from '../../../common/Axios/auth';

const Profile = () => {
    const [profile, setProfile] = useState({
        sellerID: '', // Initially empty, will be populated from token
        companyName: '',
        contactPerson: '',
        contactNumber: '',
        address: '',
        storeDescription: ''
    });
    const [isExistingSeller, setIsExistingSeller] = useState(false); // Start with false if no profile exists
    const { fetchProfile, updateProfile, addProfile } = useApi(); // Custom hook for API calls

    useEffect(() => {
        const getProfile = async () => {
            const sellerIdFromToken = getUserId(); // Get seller ID from the token
            if (sellerIdFromToken) {
                setProfile(prev => ({ ...prev, sellerID: sellerIdFromToken })); // Set sellerID in profile

                // Fetch seller profile based on sellerID
                const data = await fetchProfile(sellerIdFromToken); // API call using the sellerID from token
                if (data) {
                    setProfile(data); // Populate profile with fetched data
                    setIsExistingSeller(true); // Existing seller profile
                } else {
                    setIsExistingSeller(false); // No seller profile found, allow adding
                }
            } else {
                console.error("Seller ID not found in token");
            }
        };
        getProfile(); // Fetch profile when component mounts
    }, [fetchProfile]);

    // const handleUpdate = async (event) => {
    //     event.preventDefault();
    //     try {
    //         if (isExistingSeller) {
    //             await updateProfile(profile); // Update existing seller profile
    //         } else {
    //             await addProfile(profile); // Add new seller profile
    //         }
    //         // Optionally, show a success message or refetch the updated profile
    //     } catch (error) {
    //         console.error("Error updating profile:", error);
    //     }
    // };

    const handleUpdate = async (event) => {
        event.preventDefault();
        try {
            if (isExistingSeller) {
                await updateProfile(profile); // Update existing seller profile
                alert('Profile updated successfully!'); // Show success alert
            } else {
                await addProfile(profile); // Add new seller details
                alert('Profile added successfully!'); // Show success alert
            }
        } catch (error) {
            console.error("Error updating profile:", error);
        }
    };


    return (
        <Paper style={{ padding: '20px', maxWidth: '500px', margin: 'auto' }}>
            <Typography variant="h5">Seller Profile</Typography>
            <form onSubmit={handleUpdate}>
                {/* Read-only SellerID from token */}
                <TextField
                    label="Seller ID"
                    value={profile.sellerID || ''} // Display the sellerID fetched from token
                    InputProps={{
                        readOnly: true, // Make sellerID read-only
                    }}
                    fullWidth
                    margin="normal"
                />
                <TextField
                    label="Company Name"
                    value={profile.companyName || ''}
                    onChange={(e) => setProfile({ ...profile, companyName: e.target.value })}
                    fullWidth
                    margin="normal"
                />
                <TextField
                    label="Contact Person"
                    value={profile.contactPerson || ''}
                    onChange={(e) => setProfile({ ...profile, contactPerson: e.target.value })}
                    fullWidth
                    margin="normal"
                />
                <TextField
                    label="Contact Number"
                    value={profile.contactNumber || ''}
                    onChange={(e) => setProfile({ ...profile, contactNumber: e.target.value })}
                    fullWidth
                    margin="normal"
                />
                <TextField
                    label="Address"
                    value={profile.address || ''}
                    onChange={(e) => setProfile({ ...profile, address: e.target.value })}
                    fullWidth
                    margin="normal"
                />
                <TextField
                    label="Store Description"
                    value={profile.storeDescription || ''}
                    onChange={(e) => setProfile({ ...profile, storeDescription: e.target.value })}
                    fullWidth
                    margin="normal"
                    multiline
                    rows={4}
                />
                <Button type="submit" variant="contained" color="primary">
                    {isExistingSeller ? 'Update Profile' : 'Add Details'}
                </Button>
            </form>
        </Paper>
    );
};

export default Profile;