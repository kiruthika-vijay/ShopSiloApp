import { Link } from 'react-router-dom';

const NewArrivals = ({ products = [] }) => {
    const defaultImages = [
        "/images/arrival1.png",
        "/images/arrival2.png",
        "/images/arrival3.png",
        "/images/arrival4.png",
    ]
    return (
        <div className="mt-10 p-5">
            <div className="flex items-center mb-6">
                <h3 className="flex items-center text-lg font-bold text-utorange">
                    <span className="block h-10 w-4 bg-orange-500 rounded-md mr-2"></span>
                    Featured
                </h3>
            </div>
            <h2 className="text-3xl font-bold mb-6">New Arrivals</h2>
            <div className="flex flex-col space-y-4">
                {/* Full-length Image on Left */}
                <div className="relative w-full h-64">
                    {products.length > 0 && (
                        <img
                            src={products[0].imageURL} // Use default image if none exists
                            alt={products[0].productName}
                            className="w-full h-full object-cover"
                        />
                    )}
                    <div className="absolute bottom-0 left-0 p-4 text-white bg-black bg-opacity-50">
                        <h3 className="text-lg font-semibold">{products[0]?.productName}</h3>
                        <p className="text-sm">{products[0]?.description}</p>
                        <Link
                            to={`/product/${products[0]?.productID}`}
                            className="underline hover:text-gray-300 transition-colors"
                        >
                            Shop Now
                        </Link>
                    </div>
                </div>

                {/* Top-right Image */}
                <div className="relative w-full h-64">
                    {products.length > 1 && (
                        <img
                            src={products[1].imageURL || defaultImages[0]} // Use default image if none exists
                            alt={products[1].productName}
                            className="w-full h-full object-cover"
                        />
                    )}
                    <div className="absolute bottom-0 left-0 p-4 text-white bg-black bg-opacity-50">
                        <h3 className="text-lg font-semibold">{products[1]?.productName}</h3>
                        <p className="text-sm">{products[1]?.description}</p>
                        <Link
                            to={`/product/${products[1]?.productID}`}
                            className="underline hover:text-gray-300 transition-colors"
                        >
                            Shop Now
                        </Link>
                    </div>
                </div>

                {/* Bottom Images */}
                <div className="grid grid-cols-2 gap-4">
                    {products.slice(2, 4).map((product) => (
                        <div className="relative" key={product.productID}>
                            <img
                                src={product.imageURL || defaultImages[0]} // Use default image if none exists
                                alt={product.productName}
                                className="w-full h-40 object-cover"
                            />
                            <div className="absolute bottom-0 left-0 p-2 text-white bg-black bg-opacity-50">
                                <h3 className="text-sm font-semibold">{product.productName}</h3>
                                <p className="text-xs">{product.description}</p>
                                <Link
                                    to={`/product/${product.productID}`}
                                    className="underline hover:text-gray-300 transition-colors"
                                >
                                    Shop Now
                                </Link>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default NewArrivals;