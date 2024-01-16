import "./BySyncJobId.css"
import { useState } from 'react'
import Job from "./../Items/Job"
import JobDetails from "../Items/JobDetails"
import JobLogInvestigation from "../Items/JobLogInvestigation"

const BySyncJobId = (props) =>{

    const [workspaces, setWorkspaces] = useState(props.workspaces);
    const [jobId, setJobId] = useState();
    const [range, setRange] = useState("3");
    const [related, setRelated] = useState(0);
    const [jobs, setJobs] = useState([]);
    const [selectedJob, setSelectedJob] = useState();
    const [jobLog, setJobLog] = useState();

    const workspaceItemClickHandler = (event) => {
        const newSelectedState = event.target.className !== "w-item-selected";
        setWorkspaces(x => {
            return x.map(item => {
               return {
                ...item,
                selected: item.Id === event.target.id ? newSelectedState : item.selected
               };
            });
        });
    }

    const sortAndSetJobs = (foundJobs) => {
        const sortedByTime = foundJobs.sort((a, b) => a.TimeGenerated < b.TimeGenerated ? 1 : -1);
        setJobs(x => sortedByTime);
    } 

    const searchClickHandler = () => {
        if (jobId === null || jobId === undefined || jobId === "") {
            props.onMessage("Enter Job Id");
            return;
        }

        // clear current jobs
        setJobs(x => []); 
        setSelectedJob();
        
        props.onModal("Searching...");

        const selectedWorkspaces = workspaces.filter(x => x.selected === true).map(x => x.Id);

        if (selectedWorkspaces.length === 0) {
            props.onMessage("Select Workspace(s)");
            return;
        }

        props.client.searchJobById(jobId, selectedWorkspaces, range, related)
                    .then(foundJobs => { props.onModal(null); sortAndSetJobs(foundJobs);})                    
                    .catch(err => props.onMessage(`Search failed: ${err.message}`));                    
    }

    console.log(jobs);

    const rangeChangeHandler = (event) => {
        setRange(event.target.value);
    }

    const relatedChangeHandler = (event) => {
        setRelated(event.target.value);
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

    const jobLogInvestigationBackClickHandler = () => {
        setJobLog();        
    }

    return (
        <div className="root">
           <div className="d-header">
                <button className="btn-back" onClick={() => props.onBackClick()}>Back</button>
                <div className="d-job-id">
                    <input type="search" className="text-input" placeholder="Job Id" onChange={(event) => setJobId(event.target.value)}/>
                    <p><button className="btn-search" onClick={searchClickHandler}>Search</button></p>
                </div>
                <div className="d-search-props">
                    <p><span>Range: <select className="range-select" value={range} onChange={rangeChangeHandler}>
                        <option value="1">1 Day</option>
                        <option value="3">3 Days</option>
                        <option value="7">1 Week</option>
                        <option value="31">1 Month</option>
                        <option value="90">Max</option>
                        </select></span></p>
                    <p><span>Take: <input title="Number of previous and subsequent sessions to get" className="text-input" style={{ width: "100px" }} type="number" min={0} max={50} value={related} onChange={relatedChangeHandler}/></span></p>
                </div>
                <div className="d-search-props">
                    <div className="d-search-props" style={{padding: "0 0 8px 0"}}>
                        <label>Select AI Workspaces to search in</label>
                    </div>
                    <div className="d-workspaces">
                        {workspaces.map(x => <label key={x.Id} id={x.Id} className={x.selected ? "w-item-selected" : "w-item"} onClick={workspaceItemClickHandler}>{x.Name}</label>)}
                    </div>
                </div>
           </div>
           <div className="d-content">
                {jobLog === undefined && 
                <div className="d-job-list">
                    {jobs.map(x => <Job key={x.Properties.JobId} data={x} onJobClick={jobClickHandler} isSelected={selectedJob !== undefined && selectedJob.Properties.JobId === x.Properties.JobId}/>)}
                </div>}
                <div className="d-job-info">
                    {jobLog !== undefined && <JobLogInvestigation lines={jobLog} onBackClick={jobLogInvestigationBackClickHandler}/>}
                    {selectedJob !== undefined && jobLog === undefined && <JobDetails data={selectedJob} workspaces={workspaces} onInvestigateJobLog={investigateJobLogHandler}/>}
                </div>
           </div>
        </div>);
}

export default BySyncJobId;