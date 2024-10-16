import {
    GiSofa,
    GiFruitBowl,
    GiWhiteBook
} from "react-icons/gi";
import { PiDevicesFill } from "react-icons/pi";
import {
    FaFootballBall,
    FaGamepad,
    FaCar,
    FaKeyboard,
    FaLaptop,
    FaMobileAlt,
    FaTshirt,
    FaHome,
    FaCamera,
    FaTv,
    FaSnowflake,
    FaSoap,
    FaWind,
    FaCouch,
    FaChair,
    FaBed,
    FaUtensils,
    FaMale,
    FaFemale,
    FaChild,
    FaSmile,
    FaPaintBrush,
    FaHeartbeat,
    FaQuestionCircle, // Default icon
} from 'react-icons/fa';

const iconMap = {
    PiDevicesFill,
    GiSofa,
    FaFootballBall,
    FaGamepad,
    FaCar,
    GiFruitBowl,
    FaKeyboard,
    FaLaptop,
    FaMobileAlt,
    FaTshirt,
    FaHome,
    FaCamera,
    FaTv,
    FaSnowflake,
    FaSoap,
    FaWind,
    FaCouch,
    FaChair,
    FaBed,
    FaUtensils,
    FaMale,
    FaFemale,
    FaChild,
    FaSmile,
    FaPaintBrush,
    FaHeartbeat,
    GiWhiteBook
};

export const getIcon = (iconName) => {
    return iconMap[iconName] || FaQuestionCircle; // Return default icon if not found
};
