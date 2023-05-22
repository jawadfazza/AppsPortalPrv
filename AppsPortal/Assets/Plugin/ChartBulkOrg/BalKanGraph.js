var chart;

window.onload = function () {
    var NodesFromDatabase;
    $.ajax({
        url: '/AHD/Home/LoadOrganogram',
        method: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data3) {
            console.log(data3["Employee"]);
            NodesFromDatabase = data3["Employee"];

            OrgChart.templates.ana.plus = '<circle cx="15" cy="15" r="15" fill="#ffffff" stroke="#aeaeae" stroke-width="1"></circle>' +
                '<text text-anchor="middle" style="font-size: 18px;cursor:pointer;" fill="#757575" x="15" y="22">{collapsed-children-count}</text>';

            OrgChart.templates.invisibleGroup.padding = [20, 0, 0, 0];
            
            var editForm = function () {
                this.nodeId = null;
            };

            editForm.prototype.init = function (obj) {
                var that = this;
                this.obj = obj;
                this.editForm = document.getElementById("editForm");
                this.emailInput = document.getElementById("email");
                this.addressInput = document.getElementById("address");
                this.phone1Input = document.getElementById("phone1");
                this.phone2Input = document.getElementById("phone2");
                this.imgInput = document.getElementById("img"); 
                this.nameInput = document.getElementById("name");
                this.titleInput = document.getElementById("title");
                this.genderInput = document.getElementById("gender");
                this.nationalityInput = document.getElementById("nationality");
                this.ContractEndDateInput = document.getElementById("ContractEndDate");
                this.EmpContractTypeInput = document.getElementById("EmpContractType");
                this.EmpReqTypeInput = document.getElementById("EmpReqType");

                this.cancelButton = document.getElementById("close");

                    //this.cancelButton.addEventListener("click", function () {
                    //    that.hide();
                    //});
            };


            editForm.prototype.show = function (nodeId) {
                this.nodeId = nodeId;

                var left = document.body.offsetWidth / 2 - 150;

                this.editForm.style.left = left + "px";
                var node = chart.get(nodeId);
                if (!node) return;
                this.emailInput.innerHTML = node.email ? node.email : "";
                this.addressInput.innerHTML = node.address ? node.address : "";
                this.phone1Input.innerHTML = node.phone ? node.phone1 : "";
                this.phone2Input.innerHTML = node.phone2 ? node.phone2 : "";
                this.imgInput.src = node.img ? node.img : "#";
                this.nameInput.innerHTML = node.name ? node.name : "";
                this.titleInput.innerHTML = node.title ? node.title : "";
                this.genderInput.innerHTML = node.gender ? node.gender : "";
                this.nationalityInput.innerHTML = node.nationality ? node.nationality : "";
                this.ContractEndDateInput.innerHTML = node.ContractEndDate ? node.ContractEndDate : "";
                this.EmpContractTypeInput.innerHTML = node.EmpContractType ? node.EmpContractType : "";
                this.EmpReqTypeInput.innerHTML = node.EmpReqType ? node.EmpReqType : "";
                
                this.editForm.style.display = "block";

                OrgChart.anim(this.editForm, { opacity: 0 }, { opacity: 1 }, 300, OrgChart.anim.inOutSin);
            };

            editForm.prototype.hide = function (showldUpdateTheNode) {
                this.editForm.style.display = "none";
                this.editForm.style.opacity = 0;

            };

            /*
            chart.editUI.on('field', function (sender, args) {
                var isDeprtment = sender.node.tags.indexOf("department") != -1;
                var deprtmentFileds = ["name"];
                console.log(isDeprtment);
                if (isDeprtment && deprtmentFileds.indexOf(args.name) == -1) {
                    return false;
                }
            });*/

            chart = new OrgChart(document.getElementById("tree"), {
                template: "ana",
                enableDragDrop: true,
                collapse: {
                    level: 2,
                    allChildren: true,
                },
                zoom: {
                    speed: 130,
                    smooth: 10
                },
                scaleInitial: 0.5,
                assistantSeparation: 170,
                menu: {
                    pdfPreview: {
                        text: "Export to PDF",
                        icon: OrgChart.icon.pdf(24, 24, '#7A7A7A'),
                        onClick: preview
                    },
                    csv: {
                        text: "Save as CSV"
                    }
                },
                nodeMenu: {
                    details: {
                        text: "Details"
                    },
                    edit: {
                        text: "Edit"
                    },
                    add: {
                        text: "Add"
                    },
                    remove: {
                        text: "Remove"
                    }
                },
                align: OrgChart.ORIENTATION,
                toolbar: {
                    fullScreen: true,
                    zoom: true,
                    fit: true,
                    expandAll: true
                },
                nodeBinding: {
                    field_0: "name",
                    field_1: "title",
                    img_0: "img"
                },
                tags: {
                    "top-management": {
                        template: "invisibleGroup",
                        subTreeConfig: {
                            orientation: OrgChart.orientation.bottom,
                            collapse: {
                                level: 1
                            }
                        }
                    },
                    "it-team": {
                        subTreeConfig: {
                            layout: OrgChart.mixed,
                            collapse: {
                                level: 1
                            }
                        },
                    },
                    "security-team": {
                        subTreeConfig: {
                            layout: OrgChart.mixed,
                            collapse: {
                                level: 1
                            }
                        },
                    },
                    "hr-team": {
                        subTreeConfig: {
                            layout: OrgChart.treeRightOffset,
                            collapse: {
                                level: 1
                            }
                        },
                    },
                    "sales-team": {
                        subTreeConfig: {
                            layout: OrgChart.treeLeftOffset,
                            collapse: {
                                level: 1
                            }
                        },
                    },
                    "seo-menu": {
                        nodeMenu: {
                            addSharholder: {
                                text: "Add new sharholder",
                                icon: OrgChart.icon.add(24, 24, "#7A7A7A"),
                                onClick: addSharholder
                            },
                            addDepartment: {
                                text: "Add new department",
                                icon: OrgChart.icon.add(24, 24, "#7A7A7A"),
                                onClick: addDepartment
                            },
                            addAssistant: {
                                text: "Add new assitsant",
                                icon: OrgChart.icon.add(24, 24, "#7A7A7A"),
                                onClick: addAssistant
                            },
                            edit: {
                                text: "Edit"
                            },
                            details: {
                                text: "Details"
                            },
                        }
                    },
                    "menu-without-add": {
                        nodeMenu: {
                            details: {
                                text: "Details"
                            },
                            edit: {
                                text: "Edit"
                            },
                            remove: {
                                text: "Remove"
                            }
                        }
                    },
                    "department": {
                        template: "group",
                        subTreeConfig: {
                            layout: OrgChart.mixed,
                            collapse: {
                                level: 1
                            }
                        },
                        nodeMenu: {
                            addManager: {
                                text: "Add new manager",
                                icon: OrgChart.icon.add(24, 24, "#7A7A7A"),
                                onClick: addManager
                            },
                            remove: {
                                text: "Remove department"
                            },
                            edit: {
                                text: "Edit department"
                            },
                            nodePdfPreview: {
                                text: "Export department to PDF",
                                icon: OrgChart.icon.pdf(24, 24, "#7A7A7A"),
                                onClick: nodePdfPreview
                            }
                        }
                    },
                    /*"group": {
                        template: "group",
                    },
                    "devs-group": {
                        subTreeConfig: {
                            columns: 2
                        }
                    },
                    "sales-group": {
                        subTreeConfig: {
                            columns: 1
                        }
                    },
                    "hrs-group": {
                        min: true,
                        subTreeConfig: {
                            columns: 2
                        }
                    },*/
                },
                nodes: NodesFromDatabase,
                editUI: new editForm()
            });
            chart.fit();
            chart.on("added", function (sender, id) {
                sender.editUI.show(id);
            });

            chart.on('drop', function (sender, draggedNodeId, droppedNodeId) {
                var draggedNode = sender.getNode(draggedNodeId);
                var droppedNode = sender.getNode(droppedNodeId);

                if (droppedNode.tags.indexOf("department") != -1 && draggedNode.tags.indexOf("department") == -1) {
                    var draggedNodeData = sender.get(draggedNode.id);
                    draggedNodeData.pid = null;
                    draggedNodeData.stpid = droppedNode.id;
                    sender.updateNode(draggedNodeData);
                    return false;
                }
            });

            
            

            chart.on('exportstart', function (sender, args) {

                args.styles = document.getElementById('myStyles').outerHTML;
            });

            function preview() {
                OrgChart.pdfPrevUI.show(chart, {
                    format: 'A4'
                });
            }

            function nodePdfPreview(nodeId) {
                OrgChart.pdfPrevUI.show(chart, {
                    format: 'A4',
                    nodeId: nodeId
                });
            }

            function addSharholder(nodeId) {
                chart.addNode({
                    id: OrgChart.randomId(),
                    pid: nodeId,
                    tags: ["menu-without-add"]
                });
            }

            function addAssistant(nodeId) {
                var node = chart.getNode(nodeId);
                var data = {
                    id: OrgChart.randomId(),
                    pid: node.stParent.id,
                    tags: ["assistant"]
                };
                chart.addNode(data);
            }

            function addDepartment(nodeId) {
                var node = chart.getNode(nodeId);
                var data = {
                    id: OrgChart.randomId(),
                    pid: node.stParent.id,
                    tags: ["department"]
                };
                chart.addNode(data);
            }

            function addManager(nodeId) {
                chart.addNode({
                    id: OrgChart.randomId(),
                    stpid: nodeId
                });
            }

            //My Work
            //this event when click on (+) thats mean collapse/expand buuton
            chart.on('expcollclick', function (sender, collapse, id, ids) {
                if (!collapse) {
                    //get Node and its childrens 
                    var clickedNode = chart.getNode(id);
                    console.log(clickedNode);
                    var lenChildrens = clickedNode.childrenIds.length;
                    console.log(lenChildrens);
                    //if Node has many childrens
                    if (lenChildrens > 5) {
                        $("#tbody tr").remove();
                        for (i = 0; i < lenChildrens; i++) {

                            var nodeId = clickedNode.childrenIds[i];
                            var Node = chart.get(nodeId);
                            var NodeNode = chart.getNode(nodeId);
                            var numOfNodes = NodeNode.childrenIds.length;
                            //put data in modal
                            var tableData = '<tr>< th scope = "row" class="flex-row" ></th ><td>' +
                                Node.id + '</td><td ><img class="img-person" src="' +
                                Node.img + '"/></td><td>' +
                                Node.name + '</td><td>' +
                                Node.title + '</td><td>';
                            if (numOfNodes > 0) {
                                var buttonRef = '<button id="btn-cl-' + Node.id + '" data="' + Node.id + '">Open</button>';
                                tableData += buttonRef;
                                tableData += '</td></tr >';

                                $("#tbody").append(tableData);

                                var idbutton = document.getElementById("btn-cl-" + Node.id);

                                //event click for button in modal
                                idbutton.onclick = function () {
                                    var NodeId2 = idbutton.getAttribute("data");
                                    var numOfNodes = clickedNode.childrenIds.length;
                                    var indexNode = clickedNode.childrenIds.findIndex(e => e == NodeId2);
                                    var arraychild = clickedNode.childrenIds.slice(0, indexNode);
                                    var arraychild2 = clickedNode.childrenIds.slice(indexNode + 1, numOfNodes);
                                    arraychild = arraychild.concat(arraychild2);
                                    chart.collapse(NodeId2, arraychild);
                                    $('#modal-container1').modal('hide');
                                }
                            } else {
                                tableData += '</td></tr >';

                                $("#tbody").append(tableData);
                            }
                        }
                        $('#modal-container1').modal('show');
                    }

                    chart.center(id, {
                        parentState: OrgChart.COLLAPSE_PARENT_NEIGHBORS,
                        childrenState: OrgChart.COLLAPSE_SUB_CHILDRENS,
                        rippleId: id
                    });
                    return false;
                }
            });

        },
    });

};
/*
function sendToController() {
    console.log(chart.config.nodes);

    var d = JSON.stringify({ data: chart.config.nodes })
    $.ajax({
        url: '/Home/saveData',
        method: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ data: d}),
        success: function (data3) {
            console.log(data3);
        },
    });
}*/