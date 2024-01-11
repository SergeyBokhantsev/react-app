import "./JobLogInvestigation.css"
import { useState } from 'react'

const JobLogInvestigation = (props) => {

    const [ filterBy, setFilterBy ] = useState("ALL");

    const getClassNameBySeverity = (line) => {
        return line.includes("FATAL TID")
                          ? "log-line-fatal"
                          : line.includes(" ERROR ")
                          ? "log-line-error"
                          : line.includes(" WARN ")
                          ? "log-line-warning"
                          : line.includes(" INFO ")
                          ? "log-line-info"
                          : line.includes(" DEBUG ")
                          ? "log-line-debug"
                          : line.includes(" TRACE ")
                          ? "log-line-trace"
                          : "log-line-info";
    }; 

    const fatalCount = props.lines.filter(x => x.includes(" FATAL ")).length;
    const errorCount = props.lines.filter(x => x.includes(" ERROR ")).length;
    const warnCount = props.lines.filter(x => x.includes(" WARN ")).length;
    const infoCount = props.lines.filter(x => x.includes(" INFO ")).length;
    const debugCount = props.lines.filter(x => x.includes(" DEBUG ")).length;
    const traceCount = props.lines.filter(x => x.includes(" TRACE ")).length;

    const backClickHandler = () => {
        props.onBackClick();
    }
    
    return (
        <div className="log-root-div">
            <div style={{"text-align": "left"}}>
                <span>
                    <button onClick={backClickHandler}>Back to Job List</button>
                    <button onClick={() => setFilterBy("ALL")} className="btn-log-filter">ALL ({props.lines.length})</button>
                    <button onClick={() => setFilterBy(" FATAL ")} className="btn-log-filter">FATAL ({fatalCount})</button>
                    <button onClick={() => setFilterBy(" ERROR ")} className="btn-log-filter">ERROR ({errorCount})</button>
                    <button onClick={() => setFilterBy(" WARN ")} className="btn-log-filter">WARN ({warnCount})</button>
                    <button onClick={() => setFilterBy(" INFO ")} className="btn-log-filter">INFO ({infoCount})</button>
                    <button onClick={() => setFilterBy(" DEBUG ")} className="btn-log-filter">DEBUG ({debugCount})</button>
                    <button onClick={() => setFilterBy(" TRACE ")} className="btn-log-filter">TRACE ({traceCount})</button>
                </span>
            </div>
            <div className="log-root-div">{props.lines.filter(x => filterBy==="ALL" || x.includes(filterBy)).map(line => <span key={Math.random()} className={getClassNameBySeverity(line)}>{line}</span>)}</div>
        </div>
    );
}

export default JobLogInvestigation;