import "./Message.css"

const Message = ({ isOpen, showCloseButton, closeModal, children }) => {
    return (
      <>
        {isOpen && (
          <div className="modal-overlay">
            <div className="modal">              
              {children}
              {showCloseButton === true && <button className="close-button" onClick={closeModal}>Close</button>}
            </div>
          </div>
        )}
      </>
    );
  };
  
  export default Message;