import { useState } from 'react'
import JobLogInvestigation from '../Items/JobLogInvestigation'
import ApiClient from '../ApiClient';
import "./BySyncDumpUrl.css"

const BySyncDumpUrl = (props) => {

    const [url, setUrl] = useState();
    const [jobLog, setJobLog] = useState();


    const urlChangeHandler = (event) => {
        setUrl(event.target.value);
    }

    const handleKeyPress = (event) => {
        if (event.key === 'Enter') {
            props.onModal("Downloading job.log");
            new ApiClient().downloadJobLog(url)
                           .then(lines => { props.onModal(null); setJobLog(lines); });
        }
    };

    return (
        <div>
            <span>
                <button onClick={() => props.onBackClick()}>Back</button>
                <input type='search' onChange={urlChangeHandler} onKeyDown={handleKeyPress} className='job-log-input' placeholder='Sync Dump Download URL'/>
            </span>
            { jobLog !== undefined && <JobLogInvestigation lines={jobLog} /> }
        </div>        
    );
    
}

export default BySyncDumpUrl;