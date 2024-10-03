import { IoSearch } from "react-icons/io5";

const SearchBox = () => {
    return (
        <div className="relative flex items-center pl-5">
            <IoSearch className="mr-2 text-gray-500" />
            <input
                type="text"
                placeholder="Search here..."
                className="border border-gray-300 rounded-lg py-2 px-4 w-full focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
        </div>
    )
}

export default SearchBox;
