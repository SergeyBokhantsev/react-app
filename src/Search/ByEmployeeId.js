import "./ByEmployeeId.css"
import { useState } from 'react'
import Job from "./../Items/Job"
import JobDetails from "../Items/JobDetails"
import JobLogInvestigation from "../Items/JobLogInvestigation"
import WorkspaceSelect from "../Items/WorkspaceSelect"
import SyncDumpInvestigation from "../Items/SyncDumpInvestigation"

const ByEmployeeId = (props) => {

    const [selectedWorkspaces, setSelectedWorkspaces] = useState([]);
    const [employeeId, setEmployeeId] = useState();
    const [searchText, setSearchText] = useState();
    const [searchThreshold, setSearchThreshold] = useState(0);
    const [range, setRange] = useState("120");
    const [jobs, setJobs] = useState([]);
    const [selectedJob, setSelectedJob] = useState();
    const [jobLog, setJobLog] = useState();
    const [syncDump, setSyncDomp] = useState();

    const selectWorkspaceIdHandler = (ids) => {
        console.log(ids);
        setSelectedWorkspaces(ids);
    }

    const sortAndSetJobs = (foundJobs) => {
        const sortedByTime = foundJobs.sort((a, b) => a.TimeGenerated < b.TimeGenerated ? 1 : -1);
        setJobs(x => sortedByTime);
    } 

    const searchClickHandler = () => {
        if (employeeId === null || employeeId === undefined || employeeId === "") {
            props.onMessage("Enter Employee Id");
            return;
        }

        // clear current jobs
        setJobs(x => []); 
        setSelectedJob();
        
        props.onModal("Searching...");

        if (selectedWorkspaces.length === 0) {
            props.onMessage("Select Workspace(s)");
            return;
        }

        props.client.searchJobsByEmployeeId(employeeId, selectedWorkspaces, range, searchText, searchThreshold)
                    .then(foundJobs => { props.onModal(null); sortAndSetJobs(foundJobs);})                    
                    .catch(err => props.onMessage(`Search failed: ${err.message}`));                    
    }

    const rangeChangeHandler = (event) => {
        setRange(event.target.value);
    }

    const jobClickHandler = (job) => {
        setJobLog(undefined);
        setSelectedJob(job);
    }

    const investigateJobLogHandler = (url) => {
        props.onModal("Downloading job.log");
        props.client.downloadJobLog(url)
             .then(lines => { props.onModal(null); setJobLog(lines); });
    }

    const investigateSyncDumpHandler = (url) => {
        props.onModal("Downloading SyncDump.xml");
        props.client.downloadSyncDump(url)
             .then(xml => { props.onModal(null); setSyncDomp(xml); });
    }

    const jobLogInvestigationBackClickHandler = () => {
        setJobLog();        
    }

    return (
        <div className="root">
           <div className="d-header">
                <button className="btn-back" onClick={() => props.onBackClick()}>Back</button>
                <div className="d-job-id">
                    <input type="search" className="text-input" placeholder="Employee Id" onChange={(event) => setEmployeeId(event.target.value)}/>
                    <p><span>
                        <input type="search" className="text-input" placeholder="Search for the text" onChange={(event) => setSearchText(event.target.value)}/>
                        <input type="number" className="text-input" style={{ width: "100px", marginLeft: "10px" }} min="0" onChange={(event) => setSearchThreshold(event.target.value)}/>
                    </span></p>
                    <p><button className="btn-search" onClick={searchClickHandler}>Search</button></p>
                </div>
                <div className="d-search-props">
                    <p><span>Range: <select className="range-select" value={range} onChange={rangeChangeHandler}>
                        <option value="30">30 minutes</option>
                        <option value="60">1 Hour</option>
                        <option value="180">3 Hours</option>
                        <option value="1440">1 Day</option>
                        <option value="4320">3 Days</option>
                        <option value="10080">1 Week</option>
                        <option value="20160">2 Weeks</option>
                        </select></span></p>
                </div>
                <div className="d-search-props">
                    <div className="d-search-props" style={{padding: "0 0 8px 0"}}>
                        <label>Select AI Workspaces to search in</label>
                    </div>
                    <WorkspaceSelect workspaces={props.workspaces} onWorkspaceSelected={selectWorkspaceIdHandler}/>
                </div>
           </div>
           <div className="d-content">
                {jobLog === undefined && 
                <div className="d-job-list">
                    {jobs.map(x => <Job key={x.Properties.JobId} data={x} onJobClick={jobClickHandler} isSelected={selectedJob !== undefined && selectedJob.Properties.JobId === x.Properties.JobId}/>)}
                </div>}
                <div className="d-job-info">
                    {jobLog !== undefined && <JobLogInvestigation lines={jobLog} onBackClick={jobLogInvestigationBackClickHandler}/>}
                    {syncDump !== undefined && <SyncDumpInvestigation xml={syncDump} onBackClick={jobLogInvestigationBackClickHandler}/>}
                    {selectedJob !== undefined && jobLog === undefined && syncDump === undefined && <JobDetails data={selectedJob} workspaces={props.workspaces} onInvestigateJobLog={investigateJobLogHandler} onInvstigateSyncDump={investigateSyncDumpHandler} />}
                </div>
           </div>
        </div>);
}

export default ByEmployeeId;