import { useEffect } from "react";
import CountUp from 'react-countup';
import { HiDotsVertical } from "react-icons/hi";
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import TrendingDownIcon from '@mui/icons-material/TrendingDown';

const DashboardBox = ({ color = ["#fff", "#ddd"], icon, value, count, grow }) => {
    // Utility function to format large numbers
    const formatNumber = (num) => {
        if (num >= 1000000) {
            return (num / 1000000).toFixed(1) + 'M'; // Millions
        } else if (num >= 1000) {
            return (num / 1000).toFixed(1) + 'K'; // Thousands
        }
        return num; // If less than 1000, return the number as is
    };

    return (
        <>
            <div className="dashboardBox" style={{
                backgroundImage: `linear-gradient(to right, ${color[0]}, ${color[1]})`
            }}>
                {
                    grow === true ?
                        <span className="chart"><TrendingUpIcon /></span>
                        :
                        <span className="chart"><TrendingDownIcon/></span>
                }
                <div className="flex align-items-center w-100 bottomEle">
                    <div className="col1">
                        <h4 className="text-white">Total {value}</h4>
                        <span className="text-white">
                            <CountUp
                                start={1}
                                end={count}
                                duration={3}
                                formattingFn={formatNumber}
                            />
                        </span>
                    </div>
                    <div className="ml-auto">
                        <span className="icon">
                            {icon ? icon : '' }
                        </span>  
                    </div>                    
                </div>
                <div className="flex justify-items-center align-items-center">
                    <h5 className="text-white mb-0 mt-0">Overall Trade</h5>
                    <span className="ml-auto toggleIcon"><HiDotsVertical/></span>
                </div>
            </div>
        </>
    )
}

export default DashboardBox;
