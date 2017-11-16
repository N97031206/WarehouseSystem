var svg;
var POINT_WIDTH = 8;
var POINT_WIDTH_OFFSET = POINT_WIDTH / 2;
var selectedParkingBlock;

function OldRectangle(parkingBlockId, x, y, width, height, parkingBlockName, StorageID) {
  var self = this,
    rect, rectData = [],
    isDown = false,
    m1, m2, isDrag = true;

  self.rectData = [{
    x: x,
    y: y
  }, {
    x: x + width,
    y: y + height
  }];

  // define prototypes
  var dragR = d3.behavior.drag().on('drag', dragRect);
  var dragC1 = d3.behavior.drag().on('drag', dragPoint1);
  var dragC2 = d3.behavior.drag().on('drag', dragPoint2);
  var dragC3 = d3.behavior.drag().on('drag', dragPoint3);
  var dragC4 = d3.behavior.drag().on('drag', dragPoint4);

  svg.on('mousedown', function() {
	if (selectedParkingBlock != null) {
	  selectedParkingBlock.selectAll('.pointC').style('visibility', 'hidden');
	  hidePropertyPanel(PARKING_BLOCK);
	}
  });
  
  self.graphicElement = svg.select('#parkingBlock_g').append('g').attr('id', parkingBlockId).attr('ParkingBlockName', parkingBlockName).attr('StorageID', StorageID)

  self.rectangleElement = self.graphicElement.append('rect')
    .attr('class', 'parkingBlock')
	.attr('x', x)
	.attr('y', y)
	.attr('width', width)
	.attr('height', height)
    .call(dragR)
    .on('mousedown', function() {
	  if (selectedParkingBlock != null) {
	    selectedParkingBlock.selectAll('.pointC').style('visibility', 'hidden');
	  }
      self.graphicElement.selectAll('.pointC').style('visibility', '');
	  selectedParkingBlock = self.graphicElement;
  	  showPropertyPanel(PARKING_BLOCK);

	  updateRect();	  
	  d3.event.stopPropagation();
    })
	.on('mouseup', function() {
	  updateParkingBlockRangeToServer(self.graphicElement, rect.attr('x'), rect.attr('y'), rect.attr('width'), rect.attr('height'));
    })
	.on('mouseover', function() {
	  d3.select(this).style('cursor', 'move');
	})
	.on('mouseout', function() {
	  d3.select(this).style('cursor', '');
	});
	
  self.pointElement1 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC1); // upper left
  self.pointElement2 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC2); // down right
  self.pointElement3 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC3); // upper right
  self.pointElement4 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC4); // down left
  
  self.graphicElement.selectAll('.pointC')
  .on('mousedown', function() {
    d3.event.stopPropagation();
  })
  .on('mouseup', function() {
    updateParkingBlockRangeToServer(self.graphicElement, rect.attr('x'), rect.attr('y'), rect.attr('width'), rect.attr('height'));
  });
  
  function updateRect() {
    rect = d3.select(self.rectangleElement[0][0]);
    rect.attr({
      x: self.rectData[1].x - self.rectData[0].x > 0 ? self.rectData[0].x : self.rectData[1].x,
      y: self.rectData[1].y - self.rectData[0].y > 0 ? self.rectData[0].y : self.rectData[1].y,
      width: Math.abs(self.rectData[1].x - self.rectData[0].x),
      height: Math.abs(self.rectData[1].y - self.rectData[0].y)
    });
	
    var point1 = d3.select(self.pointElement1[0][0]).data(self.rectData);
    point1.attr('width', POINT_WIDTH)
	  .attr('height', POINT_WIDTH)
      .attr('x', self.rectData[0].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[0].y - POINT_WIDTH_OFFSET);
    var point2 = d3.select(self.pointElement2[0][0]).data(self.rectData);
    point2.attr('width', POINT_WIDTH)
      .attr('height', POINT_WIDTH)
	  .attr('x', self.rectData[1].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[1].y - POINT_WIDTH_OFFSET);
    var point3 = d3.select(self.pointElement3[0][0]).data(self.rectData);
    point3.attr('width', POINT_WIDTH)
      .attr('height', POINT_WIDTH)
	  .attr('x', self.rectData[1].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[0].y - POINT_WIDTH_OFFSET);
    var point3 = d3.select(self.pointElement4[0][0]).data(self.rectData);
    point3.attr('width', POINT_WIDTH)
      .attr('height', POINT_WIDTH)
	  .attr('x', self.rectData[0].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[1].y - POINT_WIDTH_OFFSET);
  }

  function dragRect() {
    var e = d3.event;
    for (var i = 0; i < self.rectData.length; i++) {
      d3.select(self.rectangleElement[0][0])
        .attr('x', self.rectData[i].x += e.dx)
        .attr('y', self.rectData[i].y += e.dy);
    }
    rect.style('cursor', 'move');
    updateRect();
  }

  function dragPoint1() {
    var e = d3.event;
    d3.select(self.pointElement1[0][0])
      .attr('x', function(d) {
        return d.x += e.dx
      })
      .attr('y', function(d) {
        return d.y += e.dy
      });
    updateRect();
  }

  function dragPoint2() {
    var e = d3.event;
    d3.select(self.pointElement2[0][0])
      .attr('x', self.rectData[1].x += e.dx)
      .attr('y', self.rectData[1].y += e.dy);
    updateRect();
  }

  function dragPoint3() {
    var e = d3.event;
    d3.select(self.pointElement3[0][0])
      .attr('x', self.rectData[1].x += e.dx)
      .attr('y', self.rectData[0].y += e.dy);
    updateRect();
  }

  function dragPoint4() {
    var e = d3.event;
    d3.select(self.pointElement4[0][0])
      .attr('x', self.rectData[0].x += e.dx)
      .attr('y', self.rectData[1].y += e.dy);
    updateRect();
  }

} //end Rectangle

function Rectangle() {
  var self = this,
    rect, rectData = [],
    isDown = false,
    m1, m2, isDrag = false;

  svg.on('mousedown', function() {
    m1 = d3.mouse(this);
    if (!isDown && !isDrag) {
      self.rectData = [{
        x: m1[0],
        y: m1[1]
      }, {
        x: m1[0],
        y: m1[1]
      }];
	  
	  self.graphicElement = svg.select('#parkingBlock_g').append('g');
      self.rectangleElement = self.graphicElement.append('rect').attr('class', 'parkingBlock').call(dragR);
	  self.rectangleElement
	  .on('mousedown', function() {
	    if (selectedParkingBlock != null) {
	      selectedParkingBlock.selectAll('.pointC').style('visibility', 'hidden');
	    }
        self.graphicElement.selectAll('.pointC').style('visibility', '');
	    selectedParkingBlock = self.graphicElement;
	    updateRect();
	    d3.event.stopPropagation();
      })
	  .on('mouseup', function() {
		updateParkingBlockRangeToServer(self.graphicElement, rect.attr('x'), rect.attr('y'), rect.attr('width'), rect.attr('height'));
      });
      self.pointElement1 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC1); // upper left
      self.pointElement2 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC2); // down right
      self.pointElement3 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC3); // upper right
      self.pointElement4 = self.graphicElement.append('rect').attr('class', 'pointC').call(dragC4); // down left
	  
      self.graphicElement.selectAll('.pointC')
      .on('mousedown', function() {
        d3.event.stopPropagation();
      })
      .on('mouseup', function() {
        updateParkingBlockRangeToServer(self.graphicElement, rect.attr('x'), rect.attr('y'), rect.attr('width'), rect.attr('height'));
      });

      updateRect();
      isDrag = false;
    } else {
	  if (!isDrag) {
		// not drag, add new parking block
        addParkingBlockToServer(self.graphicElement, self.rectangleElement.attr('x'), self.rectangleElement.attr('y'), 
		  self.rectangleElement.attr('width'), self.rectangleElement.attr('height'));
		selectedParkingBlock = self.graphicElement;
	  } else {
	    if (selectedParkingBlock != null) {
	      selectedParkingBlock.selectAll('.pointC').style('visibility', 'hidden');
	    }
	  }
      isDrag = true;
	}
	
    isDown = !isDown;
  })
  .on('mousemove', function() {
    m2 = d3.mouse(this);
    if (isDown && !isDrag) {
	  //console.log(m2[0]);
      self.rectData[1] = {
        x: m2[0],
        y: m2[1]
      };
      updateRect();
    }
  })
  
  function updateRect() {
    rect = d3.select(self.rectangleElement[0][0]);
    rect.attr({
      x: self.rectData[1].x - self.rectData[0].x > 0 ? self.rectData[0].x : self.rectData[1].x,
      y: self.rectData[1].y - self.rectData[0].y > 0 ? self.rectData[0].y : self.rectData[1].y,
      width: Math.abs(self.rectData[1].x - self.rectData[0].x),
      height: Math.abs(self.rectData[1].y - self.rectData[0].y)
    });

    var point1 = d3.select(self.pointElement1[0][0]).data(self.rectData);
    point1.attr('width', POINT_WIDTH)
	  .attr('height', POINT_WIDTH)
      .attr('x', self.rectData[0].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[0].y - POINT_WIDTH_OFFSET);
    var point2 = d3.select(self.pointElement2[0][0]).data(self.rectData);
    point2.attr('width', POINT_WIDTH)
      .attr('height', POINT_WIDTH)
	  .attr('x', self.rectData[1].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[1].y - POINT_WIDTH_OFFSET);
    var point3 = d3.select(self.pointElement3[0][0]).data(self.rectData);
    point3.attr('width', POINT_WIDTH)
      .attr('height', POINT_WIDTH)
	  .attr('x', self.rectData[1].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[0].y - POINT_WIDTH_OFFSET);
    var point3 = d3.select(self.pointElement4[0][0]).data(self.rectData);
    point3.attr('width', POINT_WIDTH)
      .attr('height', POINT_WIDTH)
	  .attr('x', self.rectData[0].x - POINT_WIDTH_OFFSET)
      .attr('y', self.rectData[1].y - POINT_WIDTH_OFFSET);
  }

  var dragR = d3.behavior.drag().on('drag', dragRect);

  function dragRect() {
    var e = d3.event;
    for (var i = 0; i < self.rectData.length; i++) {
      d3.select(self.rectangleElement[0][0])
        .attr('x', self.rectData[i].x += e.dx)
        .attr('y', self.rectData[i].y += e.dy);
    }
    rect.style('cursor', 'move');
    updateRect();
  }

  var dragC1 = d3.behavior.drag().on('drag', dragPoint1);
  var dragC2 = d3.behavior.drag().on('drag', dragPoint2);
  var dragC3 = d3.behavior.drag().on('drag', dragPoint3);
  var dragC4 = d3.behavior.drag().on('drag', dragPoint4);

  function dragPoint1() {
    var e = d3.event;
    d3.select(self.pointElement1[0][0])
      .attr('x', function(d) {
        return d.x += e.dx
      })
      .attr('y', function(d) {
        return d.y += e.dy
      });
    updateRect();
  }

  function dragPoint2() {
    var e = d3.event;
    d3.select(self.pointElement2[0][0])
      .attr('x', self.rectData[1].x += e.dx)
      .attr('y', self.rectData[1].y += e.dy);
    updateRect();
  }

  function dragPoint3() {
    var e = d3.event;
    d3.select(self.pointElement3[0][0])
      .attr('x', self.rectData[1].x += e.dx)
      .attr('y', self.rectData[0].y += e.dy);
    updateRect();
  }

  function dragPoint4() {
    var e = d3.event;
    d3.select(self.pointElement4[0][0])
      .attr('x', self.rectData[0].x += e.dx)
      .attr('y', self.rectData[1].y += e.dy);
    updateRect();
  }

} //end Rectangle
