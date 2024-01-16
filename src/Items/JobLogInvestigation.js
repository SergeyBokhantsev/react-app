import "./JobLogInvestigation.css"
import { useState } from 'react'

const JobLogInvestigation = (props) => {

    const FILTER_ALL = {
        fatal: true,
        error: true,
        warn: true,
        info: true,
        debug: true,
        trace: true
    }

    const [ filterBy, setFilterBy ] = useState(FILTER_ALL);

    const getClassNameBySeverity = (line) => {
        return line.includes(" FATAL ")
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

    const doFilter = (line) => {
        switch(getClassNameBySeverity(line)) {
            case "log-line-fatal": return filterBy.fatal;
            case "log-line-error": return filterBy.error;
            case "log-line-warning": return filterBy.warn;
            case "log-line-info": return filterBy.info;
            case "log-line-debug": return filterBy.debug;
            case "log-line-trace": return filterBy.trace;
            default: throw new Error(`Unexpected severity: ${getClassNameBySeverity(line)}`);
        }
    }

    const fatalCount = props.lines.filter(x => x.includes(" FATAL ")).length;
    const errorCount = props.lines.filter(x => x.includes(" ERROR ")).length;
    const warnCount = props.lines.filter(x => x.includes(" WARN ")).length;
    const infoCount = props.lines.filter(x => x.includes(" INFO ")).length;
    const debugCount = props.lines.filter(x => x.includes(" DEBUG ")).length;
    const traceCount = props.lines.filter(x => x.includes(" TRACE ")).length;

    const isAllChecked = filterBy.fatal && filterBy.error && filterBy.warn && filterBy.info && filterBy.debug && filterBy.trace;

    const onFilterChange = (filter) => {
        switch(filter)
        {
            case "all": setFilterBy(x => ({ fatal: !isAllChecked, error:!isAllChecked, warn: !isAllChecked, info:!isAllChecked, debug:!isAllChecked, trace:!isAllChecked })); break;
            case "fatal": setFilterBy(x => ({...x, fatal: !x.fatal})); break;
            case "error": setFilterBy(x => ({...x, error: !x.error})); break;
            case "warn": setFilterBy(x => ({...x, warn: !x.warn})); break;
            case "info": setFilterBy(x => ({...x, info: !x.info})); break;
            case "debug": setFilterBy(x => ({...x, debug: !x.debug})); break;
            case "trace": setFilterBy(x => ({...x, trace: !x.trace})); break;
            default: throw new Error(`Unexpected filter: ${filter}`);
        }

        console.log(filterBy);
    }

    const getFilterButtonStyle = (filter) => {
        let checked = false;
        
        switch (filter) {
            case "all": checked = false; break;
            case "fatal": checked = filterBy.fatal; break;
            case "error": checked = filterBy.error; break;
            case "warn": checked = filterBy.warn; break;
            case "info": checked = filterBy.info; break;
            case "debug": checked = filterBy.debug; break;
            case "trace": checked = filterBy.trace; break;
            default: throw new Error(`Unexpected filter: ${filter}`);
        }

        if (checked)
            return {};
        else
            return {
                "background-color": "inherit"
        }
    }

    return (
        <div className="log-root-div">
            <div style={{"text-align": "left"}}>
                <span>
                    {props.onBackClick !== undefined && <button onClick={() => props.onBackClick()}>Back</button>}
                    <button onClick={() => onFilterChange("all")} style={getFilterButtonStyle("all")} className="btn-log-filter">ALL ({props.lines.length})</button>
                    <button onClick={() => onFilterChange("fatal")} style={getFilterButtonStyle("fatal")} className="btn-log-filter">FATAL ({fatalCount})</button>
                    <button onClick={() => onFilterChange("error")} style={getFilterButtonStyle("error")} className="btn-log-filter">ERROR ({errorCount})</button>
                    <button onClick={() => onFilterChange("warn")} style={getFilterButtonStyle("warn")} className="btn-log-filter">WARN ({warnCount})</button>
                    <button onClick={() => onFilterChange("info")} style={getFilterButtonStyle("info")} className="btn-log-filter">INFO ({infoCount})</button>
                    <button onClick={() => onFilterChange("debug")} style={getFilterButtonStyle("debug")} className="btn-log-filter">DEBUG ({debugCount})</button>
                    <button onClick={() => onFilterChange("trace")} style={getFilterButtonStyle("trace")} className="btn-log-filter">TRACE ({traceCount})</button>
                </span>
            </div>
            <div className="log-root-div">{props.lines.filter(doFilter).map(line => <span key={Math.random()} className={getClassNameBySeverity(line)}>{line}</span>)}</div>
        </div>
    );
}

export default JobLogInvestigation;