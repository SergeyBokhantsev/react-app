import "./BySyncJobId.css"
import { useState } from 'react'

const BySyncJobId = (props) =>{

    const [workspaces, setWorkspaces] = useState(props.workspaces);
    const [jobId, setJobId] = useState();
    const [range, setRange] = useState("3");
    const [related, setRelated] = useState(0);
    const [jobs, setJobs] = useState([]);

    const workspaceItemClickHandler = (event) => {
        const newSelectedState = event.target.className !== "w-item-selected";
        setWorkspaces(x => {
            return x.map(item => {
               return {
                ...item,
                selected: item.id === event.target.id ? newSelectedState : item.selected
               };
            });
        });
    }

    const searchClickHandler = () => {
        setJobs(x => []); // clear current jobs
        const selectedWorkspaces = workspaces.filter(x => x.selected === true).map(x => x.id);
        props.client.searchJobById(jobId, selectedWorkspaces, range, related)
                    .then(foundJobs => setJobs(x => foundJobs));
    }

    console.log(jobs);

    const rangeChangeHandler = (event) => {
        setRange(event.target.value);
    }

    const relatedChangeHandler = (event) => {
        setRelated(event.target.value);
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
                    <p><span>Take: <input className="text-input" style={{ width: "100px" }} type="number" min={0} max={30} value={related} onChange={relatedChangeHandler}/></span></p>
                </div>
                <div className="d-workspaces">
                    {workspaces.map(x => <label key={x.id} id={x.id} className={x.selected ? "w-item-selected" : "w-item"} onClick={workspaceItemClickHandler}>{x.name}</label>)}
                </div>
           </div>
           <div className="d-content">
                <div className="d-job-list">
                    {jobs.map(x => <p key={x.Id}><label >{x.Id}</label></p>)}
                </div>
                <div className="d-job-info">

                </div>
           </div>
        </div>);
}

export default BySyncJobId;