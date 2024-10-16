import React, { useState, useEffect } from 'react';
import { apiClient } from '../common/Axios/auth';

const AuditReportList = () => {
    const [logs, setLogs] = useState([]);
    const [selectedUser, setSelectedUser] = useState('');
    const [selectedUserName, setSelectedUserName] = useState(''); // Store the selected user's name
    const [users, setUsers] = useState([]); // Corrected variable name to 'users'
    const [isDownloadable, setIsDownloadable] = useState(false); // Manage button state

    useEffect(() => {
        fetchUsers();
    }, []);

    useEffect(() => {
        if (selectedUser !== '') {
            fetchLogs();
        } else {
            setLogs([]); // Clear logs when no user is selected
        }
    }, [selectedUser]);

    const fetchUsers = async () => {
        try {
            const response = await apiClient.get('/Users');
            const fetchedUsers = response.data.$values.map(u => ({ id: u.userID, name: u.username }));
            setUsers(fetchedUsers);
            console.log('Fetched Users:', fetchedUsers); // Log fetched users to verify structure
        } catch (error) {
            console.error('Error fetching users:', error);
        }
    };

    const fetchLogs = async () => {
        try {
            const response = await apiClient.get(`/AuditLog/user/${selectedUser}`);
            setLogs(response.data.$values);
            setIsDownloadable(response.data.$values.length > 0); // Set download state based on logs
            console.log('Fetched Logs:', response.data.$values); // Log fetched logs to verify structure
        } catch (error) {
            console.error('Error fetching logs:', error);
        }
    };

    const handleUserChange = (e) => {
        const selectedId = e.target.value; // Get the selected user ID
        console.log('Selected User ID:', selectedId); // Log selected user ID

        // Find the selected user from the users array
        const selectedUserObj = users.find((u) => u.id === selectedId);
        const selectedName = selectedUserObj ? selectedUserObj.name : ''; // Get the corresponding user name
        console.log('Selected User Name:', selectedName); // Log selected user name

        setSelectedUser(selectedId); // Update selected user ID
        setSelectedUserName(selectedName); // Update selected user name
        setIsDownloadable(false); // Reset download availability until logs are fetched
    };

    const downloadExcel = async () => {
        try {
            const response = await apiClient.get(`/AuditLog/export/${selectedUser}`, {
                responseType: 'blob',  // Ensure the response is treated as a file
            });
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'AuditLogs.xlsx');  // Specify the file name
            document.body.appendChild(link);
            link.click();
            link.remove();
        } catch (error) {
            console.error('Error downloading Excel:', error);
        }
    };

    return (
        <div className="container mx-auto px-4 py-6">
            {users.length > 0 && (
                <div>
                    <h2 className="text-2xl font-bold mb-4">Audit Logs</h2>
                    <div className="flex justify-between mb-4">
                        <div className="flex">
                            <select
                                value={selectedUser}
                                className="border rounded p-2"
                                onChange={handleUserChange} // Use the new handler
                            >
                                <option value="" disabled>Select a user</option>
                                {users.map((item) => (
                                    <option key={item.id} value={item.id}>
                                        {item.name}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <button
                            onClick={downloadExcel}
                            className={`bg-blue-500 text-white p-2 rounded ml-4 ${!isDownloadable ? 'opacity-50 cursor-not-allowed' : ''}`}
                            disabled={!isDownloadable} // Disable the button if no logs are available
                            >
                            {isDownloadable
                                ? `Download Excel ` // Use the updated state for the button text
                                : 'Select a user to download Excel'}
                        </button>
                </div>
                </div>
    )
}

{
    logs.length > 0 ? (
        <div className="container mx-auto p-4">
            <h2 className="text-3xl font-bold mb-6 text-center">Audit Logs</h2>
            <table className="min-w-full bg-white border border-gray-200">
                <thead>
                    <tr className="bg-gray-100 border-b">
                        <th className="py-2 px-4 text-left">User Name</th>
                        <th className="py-2 px-4 text-left">Action</th>
                        <th className="py-2 px-4 text-left">Date</th>
                    </tr>
                </thead>
                <tbody>
                    {logs.map((log) => (
                        <tr key={log.logId} className="border-b hover:bg-gray-50">
                            <td className="py-2 px-4">{users.find((u) => u.id === log.userId)?.name}</td>
                            <td className="py-2 px-4">{log.action}</td>
                            <td className="py-2 px-4">
                                {new Date(log.timestamp).toLocaleString()}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    ) : (
    <p>No logs found.</p>
)
}
        </div >
    );
};

export default AuditReportList;