import "./Job.css"

const Job = (props) => {

    let statusColor;

    switch (props.data.Name) {
        case "Jobs/Finished": statusColor="green"; break;
        case "Jobs/Error": statusColor="crimson"; break;
        case "Jobs/Skipped": statusColor="gray"; break;
        default: statusColor="darkorange"; break;
    }

    const time = new Date(props.data.TimeGenerated).toISOString().slice(0, -5).replace('T', ' ') + " UTC";

    const message = props.data.Properties.Message;

    const rootDivClassName = props.isSelected === true ? "rowww-selected" : "job-root";
    const rowIdClassName = props.data.IsTarget ? "row-id-target" : "row-id";

    return (
        <div className={rootDivClassName} onClick={() => props.onJobClick(props.data)}>            
            <p className="rowww"><span><label>{time}</label> | <label style={{"background-color": statusColor, padding: "0 10px 0 10px  "}} >{props.data.Name}</label></span></p>
            <label className={rowIdClassName}>{props.data.Properties.JobId}</label>
            { message !== undefined && <label className="job-message" title={message}>{message}</label> }
            {/* <p className="rowww"><a href={props.data.Properties.JobLogUrl}>Download Log</a></p> */}
            <p></p>
        </div>
    );
}

export default Job;