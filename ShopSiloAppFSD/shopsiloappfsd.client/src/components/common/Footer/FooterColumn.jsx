// FooterColumn.js
import React from 'react';
import { Link } from 'react-router-dom';

const FooterColumn = ({ title, content }) => {
    return (
        <div className="flex flex-col text-neutral-50">
            <h3 className="text-xl font-medium leading-snug">{title}</h3>
            <ul className="flex flex-col mt-6 text-base">
                {content.map((item, index) => (
                    <li key={index} className={index > 0 ? "mt-4" : ""}>
                        <Link
                            to={item.link}
                            className="text-neutral-50 hover:underline"
                            target={item.link.startsWith('http') ? '_blank' : '_self'}
                            rel={item.link.startsWith('http') ? 'noopener noreferrer' : undefined}
                        >
                            {item.text}
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default FooterColumn;
