import React from "react";

const Test = () => {
  return (
    <div style={{ width: "600px", height: "400px", border: "solid 1px red" }}>
      <div style={{ display: "flex", flexDirection: "row", height: "100%", width: "100%" }}>
        <div className="sider">sidersidersidersidersider</div>

        <div
          className="layout"
          style={{ border: "solid 1px black", overflow: "auto", flex: 1, width: 0, margin: 5, display: "flex", flexDirection: "column" }}
        >
          <div className="content" style={{ flex: "auto" }}>
            <div style={{ border: "solid 1px green" }}>flexDirection</div>
            <div style={{ display: "flex" }}>
              <div style={{ overflow: "auto", flex: 1, width: 0, border: "solid 1px blue", margin: 5 }}>
                <div style={{ border: "solid 1px blue", width: 800, height: 900, margin: 5 }}>
                  ksjd hflkajsdh flkjashd lfkja sd fjashd fajh
                </div>
              </div>
            </div>
          </div>
          <div className="footer" style={{ border: "solid 1px yellow" }}>
            ksjd hflkajsdh flkjashd lfkja sd fjashd fajh
          </div>
        </div>
      </div>
    </div>
  );
};

export default Test;
