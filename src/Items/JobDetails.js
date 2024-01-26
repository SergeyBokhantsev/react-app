import React from "react";
import "./JobDetails.css"

const JobDetails = (props) => {
    
    const propsToIgnore = ["JobLogUrl", "IsTarget", "Properties", "Measurements", "ParentId", "OperationId", "AppRoleName", "ClientType", "ClientIP", "ResourceGUID", "IKey", "SDKVersion", "ItemCount", "SourceSystem", "Type", "_ResourceId"];

    const rootProps = [];

    for (let [key, value] of Object.entries(props.data)) {
        if (value !== null && value !== undefined && value !=="" && !propsToIgnore.includes(key)){

            if (key === "WorkspaceId") {
                value = (props.workspaces.filter(x => x.Id === value)[0]).Name;
            }
            rootProps.push({key, value});
        }
    }

    const properties = [];

    if (typeof(props.data.Properties) === "object") {
        for (let [key, value] of Object.entries(props.data.Properties)) {
            if (value !== null && value !== undefined && value !=="" && !propsToIgnore.includes(key)){
                properties.push({key, value});
            }
        }
    }

    const measurements = [];

    if (typeof(props.data.Measurements) === "object") {
        for (let [key, value] of Object.entries(props.data.Measurements)) {
            if (value !== null && value !== undefined && value !=="" && !propsToIgnore.includes(key)){
                measurements.push({key, value});
            }
        }
    }

    const jobLogClickHandler = () => {
        props.onInvestigateJobLog(props.data.Properties.JobLogUrl);
    }

    const syncDumpClickHandler = () => {
        props.onInvstigateSyncDump(props.data.Properties.JobLogUrl);
    }

    return (
        <div>
            <label>Base Props</label>
            <div className="props-div">
                {rootProps.map(x => <span key={x.key}><label className="key-label">{x.key}: </label><label>{x.value}</label></span>)}
                <span>
                    <a className="download-link" href={props.data.Properties.JobLogUrl}>Download Sync Dump</a>
                    <button className="button-small" onClick={jobLogClickHandler}>Investigate job.log</button>
                    <button className="button-small" onClick={syncDumpClickHandler}>Investigate SyncDump.xml</button>
                </span>
            </div>
            <label>Custom Props</label>
            <div className="props-div">
                {properties.map(x => <span key={x.key}><label className="key-label">{x.key}: </label><label>{x.value}</label></span>)}
            </div>
            <label>Custom Measurements</label>
            <div className="props-div">
                {measurements.map(x => <span key={x.key}><label className="key-label">{x.key}: </label><label>{x.value}</label></span>)}
            </div>
        </div>
    );
}

export default JobDetails;