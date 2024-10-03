import React, { useState, useEffect } from 'react';
import { apiClient } from '../common/Axios/auth';

const AuditReportList = () => {
    const [logs, setLogs] = useState([]);
    const [selectedUser, setSelectedUser] = useState('');
    const [user, setUser] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(5);

    useEffect(() => {
        if (user.length > 0) {
            setSelectedUser(user[0].id);
        } else {
            fetchUsers();
        }
    }, [user]);

    useEffect(() => {
        if (selectedUser !== '') {
            fetchLogs();
        }
    }, [selectedUser]);

    const fetchUsers = async () => {
        try {
            const response = await apiClient.get('/Users');
            setUser(response.data.$values.map(u => ({ id: u.userID, name: u.username })));
        } catch (error) {
            console.error('Error fetching users:', error);
        }
    };

    const fetchLogs = async () => {
        try {
            const response = await apiClient.get(`/AuditLog/user/${selectedUser}`);
            setLogs(response.data.$values);
        } catch (error) {
            console.error('Error fetching logs:', error);
        }
    };

    return (
        <div className="container mx-auto px-4 py-6">
            {user.length > 0 && (
                <div>
                    <h2 className="text-2xl font-bold mb-4">Audit Logs</h2>
                    <div className="flex justify-between mb-4">
                        <div className="flex">
                            <select
                                value={selectedUser}
                                className="border rounded p-2"
                                onChange={(e) => setSelectedUser(e.target.value)}
                            >
                                {user.map((item) => (
                                    <option key={item.id} value={item.id}>
                                        {item.name}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                </div>
            )}

            {logs.length > 0 ? (
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
                                    <td className="py-2 px-4">{user.find((u) => u.id === log.userId)?.name}</td>
                                    <td className="py-2 px-4">{log.action}</td>
                                    {/* Use the Date object to format the timestamp */}
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
            )}
        </div>
    );
};

export default AuditReportList;