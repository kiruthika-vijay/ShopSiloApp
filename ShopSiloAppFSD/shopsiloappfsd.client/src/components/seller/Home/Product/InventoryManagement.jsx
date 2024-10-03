import React, { useEffect, useState } from 'react';
import axios from 'axios';
import {
    Button,
    TextField,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Snackbar,
    Alert,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
    Typography,
    Card,
    IconButton,
} from '@mui/material';
import { apiClient } from '../../../common/Axios/auth';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { green, yellow, red } from '@mui/material/colors';

const InventoryManagement = () => {
    const [inventories, setInventories] = useState([]);
    const [quantity, setQuantity] = useState('');
    const [productId, setProductId] = useState('');
    const [selectedInventory, setSelectedInventory] = useState(null);
    const [open, setOpen] = useState(false);
    const [snackMessage, setSnackMessage] = useState('');
    const [snackOpen, setSnackOpen] = useState(false);

    useEffect(() => {
        fetchInventories();
    }, []);

    const fetchInventories = async () => {
        try {
            const response = await apiClient.get('/Inventory/seller');
            setInventories(Array.isArray(response.data.$values) ? response.data.$values : []);
        } catch (error) {
            console.error('Error fetching inventories:', error);
            setInventories([]);
        }
    };

    const handleOpenDialog = (inventory) => {
        setSelectedInventory(inventory);
        setQuantity(inventory ? inventory.quantity : '');
        setProductId(inventory ? inventory.productID : '');
        setOpen(true);
    };

    const handleCloseDialog = () => {
        setOpen(false);
        setSelectedInventory(null);
        setQuantity('');
        setProductId('');
    };

    const handleUpdateInventory = async () => {
        try {
            const updatedInventory = { ...selectedInventory, quantity: parseInt(quantity) };
            await apiClient.put('/Inventory', updatedInventory);
            fetchInventories();
            setSnackMessage('Inventory updated successfully!');
            handleCloseDialog();
            setSnackOpen(true);
        } catch (error) {
            console.error('Error updating inventory:', error);
        }
    };

    const handleAddInventory = async () => {
        try {
            const newInventory = {
                productID: productId,
                quantity: parseInt(quantity),
            };
            await apiClient.post('/Inventory', newInventory); // Send a POST request to add inventory
            fetchInventories();
            setSnackMessage('Inventory added successfully!');
            handleCloseDialog();
            setSnackOpen(true);
        } catch (error) {
            console.error('Error adding inventory:', error);
            setSnackMessage('Failed to add inventory.'); // Show error message
            setSnackOpen(true);
        }
    };

    const handleDeleteInventory = async (id) => {
        try {
            await apiClient.delete(`/Inventory/${id}`);
            fetchInventories();
            setSnackMessage('Inventory deleted successfully!');
            setSnackOpen(true);
        } catch (error) {
            console.error('Error deleting inventory:', error);
        }
    };

    const getStatusColor = (inventory) => {
        if (inventory.quantity === 0 || inventory.isActive === false) return { color: red[500], text: 'Not Available', textColor: red[900] }; // Red
        if (inventory.quantity < 50) return { color: yellow[700], text: 'Low Stock', textColor: yellow[900] }; // Dark Yellow
        return { color: green[500], text: 'Available', textColor: green[900] }; // Green
    };

    const formatDate = (dateString) => {
        const options = { year: 'numeric', month: 'short', day: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    };

    return (
        <div style={{ padding: '20px' }}>
            <Typography variant="h4" gutterBottom>
                Inventory Management
            </Typography>
            <Card elevation={3} style={{ padding: '20px', marginBottom: '20px', backgroundColor: '#c3dee0' }}>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={() => handleOpenDialog(null)}
                    style={{ borderRadius: '20px', marginBottom: '20px' }}
                >
                    Add Inventory
                </Button>
                <TableContainer component={Paper} style={{ boxShadow: '0 2px 10px rgba(0,0,0,0.1)' }}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell style={{ fontWeight: 'bold' }}>Item Name</TableCell>
                                <TableCell style={{ fontWeight: 'bold' }}>Quantity</TableCell>
                                <TableCell style={{ fontWeight: 'bold' }}>Product ID</TableCell>
                                <TableCell style={{ fontWeight: 'bold' }}>Price</TableCell>
                                <TableCell style={{ fontWeight: 'bold' }}>Date Added</TableCell>
                                <TableCell style={{ fontWeight: 'bold' }}>Options</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {Array.isArray(inventories) && inventories.length > 0 ? (
                                inventories.map((inventory) => (
                                    <TableRow key={inventory.inventoryID}>
                                        <TableCell>
                                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                                <img
                                                    src={inventory.productImageUrl}
                                                    alt="Product"
                                                    style={{
                                                        width: '100px',
                                                        height: '100px',
                                                        borderRadius: '8px',
                                                        objectFit: 'cover',
                                                        marginRight: '10px',
                                                    }}
                                                />
                                                <div>
                                                    <Typography variant="body1">{inventory.productName}</Typography>
                                                    <Typography variant="body2" color="textSecondary">Category: {inventory.category}</Typography>
                                                    <Typography variant="body2" color="textSecondary">PID: 000{inventory.productID}</Typography>
                                                </div>
                                            </div>
                                        </TableCell>
                                        <TableCell>
                                            <Card style={{
                                                padding: '3px',
                                                width: '100%',
                                                alignItems: 'center',
                                                backgroundColor: getStatusColor(inventory).color,
                                                color: '#fff',
                                            }}>
                                                <Typography variant="h6" style={{ margin: '0 10px' }}>
                                                    {inventory.quantity}
                                                </Typography>
                                            </Card>
                                            <Typography
                                                variant="body2"
                                                style={{
                                                    width: '100%',
                                                    backgroundColor: getStatusColor(inventory).color + '60', // Slightly lighter shade
                                                    color: getStatusColor(inventory).textColor,
                                                    fontWeight: '600',
                                                    textAlign: 'center',
                                                    borderRadius: '4px',
                                                    padding: '4px 0',
                                                    marginTop: '5px',
                                                }}
                                            >
                                                {getStatusColor(inventory).text}
                                            </Typography>
                                        </TableCell>
                                        <TableCell>{inventory.productID}</TableCell>
                                        <TableCell>{inventory.price}</TableCell>
                                        <TableCell>{formatDate(inventory.dateAdded)}</TableCell>
                                        <TableCell>
                                            <IconButton onClick={() => handleDeleteInventory(inventory.inventoryID)} color="secondary">
                                                <DeleteIcon />
                                            </IconButton>
                                            <IconButton onClick={() => handleOpenDialog(inventory)} color="primary">
                                                <EditIcon />
                                            </IconButton>
                                        </TableCell>
                                    </TableRow>
                                ))
                            ) : (
                                <TableRow>
                                    <TableCell colSpan={6}>No inventory data available.</TableCell>
                                </TableRow>
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Card>

            <Dialog open={open} onClose={handleCloseDialog}>
                <DialogTitle>{selectedInventory ? 'Update Inventory' : 'Add Inventory'}</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        {selectedInventory ? 'Update the inventory quantity.' : 'Enter the inventory quantity.'}
                    </DialogContentText>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Product ID"
                        type="text"
                        fullWidth
                        value={productId}
                        onChange={(e) => setProductId(e.target.value)}
                        disabled={selectedInventory !== null} // Disable when updating
                    />
                    <TextField
                        margin="dense"
                        label="Quantity"
                        type="number"
                        fullWidth
                        value={quantity}
                        onChange={(e) => setQuantity(e.target.value)}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseDialog} color="primary">
                        Cancel
                    </Button>
                    <Button
                        onClick={selectedInventory ? handleUpdateInventory : handleAddInventory}
                        color="primary"
                    >
                        {selectedInventory ? 'Update' : 'Add'}
                    </Button>
                </DialogActions>
            </Dialog>

            <Snackbar open={snackOpen} autoHideDuration={6000} onClose={() => setSnackOpen(false)}>
                <Alert onClose={() => setSnackOpen(false)} severity="success" sx={{ width: '100%' }}>
                    {snackMessage}
                </Alert>
            </Snackbar>
        </div>
    );
};

export default InventoryManagement;
