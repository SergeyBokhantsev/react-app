import XMLViewer from 'react-xml-viewer'

const SyncDumpInvestigation = (props) => {
    return (
      <div>
        <XMLViewer collapsible={true} xml={props.xml} />
      </div>
    )
}

export default SyncDumpInvestigation;