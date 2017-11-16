		var parkingBlockData = {};
		var CART_WIDTH = 12;
		var CART_HEIGHT = 14;
		var PARKING_BLOCK_MARGIN = 2;
		
		var CART_TIME_STATUS_COLOR_NORMAL = '#b9cde5';
		var CART_TIME_STATUS_COLOR_HALF_OVERTIME = '#ffff00';
		var CART_TIME_STATUS_COLOR_OVERTIME = '#ff0000';
		var CART_TIME_STATUS_COLOR_LOW_POWER = '#000000';
		
		// define time status of the cart
		var CART_TIME_STATUS_NORMAL = 0;
		var CART_TIME_STATUS_HALF_OVERTIME = 1;
		var CART_TIME_STATUS_OVERTIME = 2;
		var CART_TIME_STATUS_LOW_POWER = 3;
		
		var CART_STATUS_USED = 'USED';
		var CART_STATUS_IDLE = 'IDLE';
		var CART_STATUS_TERMINATED = 'TERMINATED';
		
		var CART_SCHEDULE_PERIOD = 'schedulePeriod';
		var CART_SCHEDULE_BEFORE = 'scheduleBefore';
		var CART_SCHEDULE_AFTER = 'scheduleAfter';
		
		var carrierLotData = {};
		
		function updateCartStatus(cartNumber, status) {
			var type = cartNumber.substring(0, 1);
			switch (type) {
				case 'D':
				case 'W':
				case 'T':
				case 'Q':
				case 'C':
					switch (status) {
						case CART_TIME_STATUS_NORMAL:
							$('#cart_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_NORMAL);
							break;
						case CART_TIME_STATUS_OVERTIME:
							$('#cart_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_OVERTIME);
							break;
						case CART_TIME_STATUS_HALF_OVERTIME:
							$('#cart_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_HALF_OVERTIME);
							break;
						case CART_TIME_STATUS_LOW_POWER:
							$('#cart_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_LOW_POWER);
							break;
					}
					break;
				case 'B':
				case 'A':
					switch (status) {
						case CART_TIME_STATUS_NORMAL:
							$('#cartBackgroud_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_NORMAL);
							break;
						case CART_TIME_STATUS_OVERTIME:
							$('#cartBackgroud_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_OVERTIME);
							break;
						case CART_TIME_STATUS_HALF_OVERTIME:
							$('#cartBackgroud_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_HALF_OVERTIME);
							break;
						case CART_TIME_STATUS_LOW_POWER:
							$('#cartBackgroud_' + cartNumber).attr('fill', CART_TIME_STATUS_COLOR_LOW_POWER);
							break;
					}
					break;
			}
		}
		
       
		//function showLotData(cartNumber, index, lotIndex) {
		//	$('#lotInfoPanel').removeClass('hide');
		//	//console.log(carrierLotData[cartNumber]);
		//	var localLotData = carrierLotData[cartNumber]['CarrierLotData'][index]['LotList'][lotIndex];
		//	$('#Lot').text(localLotData['Lot']);
		//	$('#DeviceDescr').text(localLotData['DeviceDescr']);	
		//	$('#LastOutTime').text(localLotData['LastOutTime']);
		//	$('#OperationNo').text(localLotData['OperationNo']);
		//	$('#Quantity').text(localLotData['Quantity']);
		//	$('#WaitStatus').text(localLotData['WaitStatus']);
		//	$('#MaxWaitTimeHour').text(localLotData['MaxWaitTimeHour']);
		//	$('#WaitTime').text(localLotData['WaitTime']);
		//	$('#ColorName').text(localLotData['ColorName']);
		//	$('#CustomerName').text(localLotData['CustomerName']);
		//	$('#Factory').text(localLotData['Factory']);
		//	$('#ScheduleDate').text(localLotData['ScheduleDate']);
		//	$('#SalesAssistant').text(localLotData['SalesAssistant']);
		//}

		function showLotData(cartNumber, index, lotIndex) {
		    $('#lotInfoPanel').removeClass('hide');

		    //var localLotData = carrierLotData[cartNumber]['CarrierLotData'][index]['LotList'][lotIndex];
		    var localLotData = carrierLotData[cartNumber][index];

		    $('#Lot').text(localLotData['PLot']);
		    $('#WKNo').text(localLotData['WKNo']);
		    $('#LotNo').text(localLotData['LotNo']);
		    $('#Order').text(localLotData['Order']);
		    $('#Fabric').text(localLotData['Fabric']);
		    $('#Color').text(localLotData['ColorCode'] + localLotData['Color']);
		    $('#Width').text(localLotData['Width']);
		    $('#YdWt').text(localLotData['YdWt']);
		    $('#Weight').text(localLotData['Weight']);
		    $('#Length').text(localLotData['Length']);
		    $('#PNo').text(localLotData['PNo']);
		    $('#Date').text(localLotData['Date']);
		    $('#InternalOrder').text(localLotData['InternalOrder']);
		    
		}
		
		function drawPopover(drawId, cartNumber, contentString) {
			$(drawId).popover({
				trigger: 'manual',
				html: true,
				title: '<h4>棧板編號：' + cartNumber + '</h4><br>布疋批號明細',
				content: '<div class="container">' + contentString + '</div>',
				container: 'body',
				animation: false
			}).on('mouseenter', function() {
				var _this = this;
				$(this).popover('show');
				$('.popover').on('mouseleave', function() {
					$(_this).popover('hide');
				});
			}).on('mouseleave', function() {
				var _this = this;
				setTimeout(function() {
					if (!$('.popover:hover').length) {
						$(_this).popover('hide');
					}
				}, 300);
			});
		}
		
		function clearPopover() {
			var popoverArray = $('.popover');
			$.each (popoverArray, function(index, value){
				$(value).popover('hide');
			});
		}
				
		function drawDWTTypeCart(fieldId, cartLayer, cartNumber, x, y) {
			// create a selected rectangle tag
			var rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
			rect.setAttributeNS(null, 'id', 'cart_' + cartNumber);
			rect.setAttributeNS(null, 'width', '8');
			rect.setAttributeNS(null, 'height', '10');
			rect.setAttributeNS(null, 'x', x);
			rect.setAttributeNS(null, 'y', y);
			rect.setAttributeNS(null, 'rx', 2);
			rect.setAttributeNS(null, 'ry', 2);
			rect.setAttributeNS(null, 'fill', CART_TIME_STATUS_COLOR_NORMAL);
			rect.setAttributeNS(null, 'stroke', '#0000ff');
			rect.setAttributeNS(null, 'stroke-width', 2);
			$('#' + fieldId + '_svg .svg-pan-zoom_viewport ' + cartLayer).append(rect);
		}
		
		// round cart
		function drawQTypeCart(fieldId, cartLayer, cartNumber, x, y) {
			// create a selected circle tag
			var circle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
			circle.setAttributeNS(null, 'id', 'cart_' + cartNumber);
			circle.setAttributeNS(null, 'cx', x + 4);
			circle.setAttributeNS(null, 'cy', y + 5);
			circle.setAttributeNS(null, 'r', 4);
			circle.setAttributeNS(null, 'fill', CART_TIME_STATUS_COLOR_NORMAL);
			circle.setAttributeNS(null, 'stroke', '#0000ff');
			circle.setAttributeNS(null, 'stroke-width', 2);
			$('#' + fieldId + '_svg .svg-pan-zoom_viewport ' + cartLayer).append(circle);	
		}
		
		// 
		function drawBigATypeCart(fieldId, cartLayer, cartNumber, x, y) {
			// create g tag
			var g = document.createElementNS('http://www.w3.org/2000/svg','g');
			g.setAttributeNS(null, 'id', '#cart_' + cartNumber + '_g');
            $('#' + fieldId + '_svg .svg-pan-zoom_viewport ' + cartLayer).append(g);
			
			// create a selected rectangle tag
			var rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
			rect.setAttributeNS(null, 'id', 'cartBackgroud_' + cartNumber);
			rect.setAttributeNS(null, 'width', '10');
			rect.setAttributeNS(null, 'height', '12');
			rect.setAttributeNS(null, 'x', x - 1);
			rect.setAttributeNS(null, 'y', y - 1);
			rect.setAttributeNS(null, 'rx', 2);
			rect.setAttributeNS(null, 'ry', 2);
			rect.setAttributeNS(null, 'fill', CART_TIME_STATUS_COLOR_NORMAL);
			$(g).append(rect);
			
			// big a cart
			var text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
			text.setAttributeNS(null, 'id', 'cart_' + cartNumber);
			text.setAttributeNS(null, 'x', x);
			text.setAttributeNS(null, 'y', y + 10);
			text.setAttributeNS(null, 'fill', '#00b050');
			text.setAttributeNS(null, 'stroke', '#00b050');
			text.setAttributeNS(null, 'stroke-width', 1);
			text.setAttributeNS(null, 'cursor', 'default');
			var textNode = document.createTextNode('A');
			text.appendChild(textNode);
			$(g).append(text);
		}
		
		function drawSmallATypeCart(fieldId, cartLayer, cartNumber, x, y) {
			// create g tag
			var g = document.createElementNS('http://www.w3.org/2000/svg','g');
			g.setAttributeNS(null, 'id', '#cart_' + cartNumber + '_g');
            $('#' + fieldId + '_svg .svg-pan-zoom_viewport ' + cartLayer).append(g);
			
			// create a selected rectangle tag
			var rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
			rect.setAttributeNS(null, 'id', 'cartBackgroud_' + cartNumber);
			rect.setAttributeNS(null, 'width', '10');
			rect.setAttributeNS(null, 'height', '12');
			rect.setAttributeNS(null, 'x', x - 1);
			rect.setAttributeNS(null, 'y', y - 1);
			rect.setAttributeNS(null, 'rx', 2);
			rect.setAttributeNS(null, 'ry', 2);
			rect.setAttributeNS(null, 'fill', CART_TIME_STATUS_COLOR_NORMAL);
			$(g).append(rect);

			// small a cart
			var text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
			text.setAttributeNS(null, 'id', 'cart_' + cartNumber);
			text.setAttributeNS(null, 'x', x);
			text.setAttributeNS(null, 'y', y + 9);
			text.setAttributeNS(null, 'fill', '#00b050');
			text.setAttributeNS(null, 'stroke', '#00b050');
			text.setAttributeNS(null, 'stroke-width', 1);
			text.setAttributeNS(null, 'cursor', 'default');
			var textNode = document.createTextNode('a');
			text.appendChild(textNode);
			$(g).append(text);
		}
		
		function drawCTypeCart(fieldId, cartLayer, cartNumber, x, y) {
			var newX = 0;
			var newY = 0;
			// create a selected circle tag
			var path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
			path.setAttributeNS(null, 'id', 'cart_' + cartNumber);
			newX = x + 2;
			var pathCommand = 'M ' + newX + ' ' + y;
			newX = newX + 4;
			pathCommand = pathCommand + ' L ' + newX + ' ' + y;
			newX = newX + 2;
			newY = y + 10;
			pathCommand = pathCommand + ' L ' + newX + ' ' + newY;
			newX = newX - 8;
			pathCommand = pathCommand + ' L ' + newX + ' ' + newY;
			pathCommand = pathCommand + ' Z';
			path.setAttributeNS(null, 'd', pathCommand);
			path.setAttributeNS(null, 'fill', CART_TIME_STATUS_COLOR_NORMAL);
			path.setAttributeNS(null, 'stroke', '#00b050');
			path.setAttributeNS(null, 'stroke-width', 2);
			path.setAttributeNS(null, 'stroke-linejoin', 'round');
			$('#' + fieldId + '_svg .svg-pan-zoom_viewport ' + cartLayer).append(path);			
		}
		
		function setCartBlink(cartNumber) {
			//$('#cart_' + cartNumber).attr({'class':'blinkClass', 'stroke':'#ffff00'});
			$('#cart_' + cartNumber).attr({'class':'blinkClass'});
		}
	
		function findParkingBlock(fieldId, parkingBlockId) {
			var result = null;
			if (fieldId in parkingBlockData) {				
				var fieldParkingBlockData = parkingBlockData[fieldId];
				for (var i = 0; i < fieldParkingBlockData.length; i++) {
					if (fieldParkingBlockData[i]['ParkingBlockID'] == parkingBlockId) {
						result = fieldParkingBlockData[i];
						if (!('cartCount' in result)) {
							// set default cart count
							result['cartCount'] = 0;
							//console.log(result);
						}
						break;
					}
				}
			}
			return result;
		}
		
		function drawCart(fieldId, parkingBlockId, cartLayer, cartNumber) {
			var parkingBlock = findParkingBlock(fieldId, parkingBlockId);
			if (parkingBlock != null) {
				var parkingBlockX = parseInt(parkingBlock['X']) + PARKING_BLOCK_MARGIN;
				var parkingBlockY = parseInt(parkingBlock['Y']) + PARKING_BLOCK_MARGIN;

				// find column number of the parking block
				var columnCount = parseInt(parseInt(parkingBlock['Width']) / CART_WIDTH);
				
				// get last row and column
				var row = parseInt(parseInt(parkingBlock['cartCount']) / columnCount); 
				var column = parseInt(parkingBlock['cartCount']) % columnCount;
				//console.log('row = ' + row + ', column = ' + column);
			
				var x, y;
				x = parkingBlockX + column * CART_WIDTH;
				y = parkingBlockY + row * CART_HEIGHT;
				
				drawDWTTypeCart(fieldId, cartLayer, cartNumber, x, y);

				////console.log(cartNumber);
				//var cartType = cartNumber.substring(0, 1);				
				////console.log(cartType);
				//switch (cartType) {
				//	case 'D':
				//	case 'W':
				//	case 'T':
				//		drawDWTTypeCart(fieldId, cartLayer, cartNumber, x, y);
				//		break;
				//	case 'Q':
				//		drawQTypeCart(fieldId, cartLayer, cartNumber, x, y);
				//		break;
				//	case 'B':
				//		drawBigATypeCart(fieldId, cartLayer, cartNumber, x, y);
				//		break;
				//	case 'A':
				//		drawSmallATypeCart(fieldId, cartLayer, cartNumber, x, y);
				//		break;						
				//	case 'C':
				//		drawCTypeCart(fieldId, cartLayer, cartNumber, x, y);
				//		break;						
				//}
				parkingBlock['cartCount'] = parkingBlock['cartCount'] + 1;
			}
		}
	
		//function addCartToParkingBlockWithScheduleDate(fieldId, parkingBlockId, cartNumber, dateQueryType, dateStart, dateEnd) {
		//	var lotString = '';
		//	var finalWaitStatus = CART_TIME_STATUS_NORMAL;
		//	$.post('http://172.16.3.180/EverestAPI04/Automation/GetCarrierDataAndLots',
		//		{Carrier: cartNumber},
		//		function (data) {
		//			carrierLotData[cartNumber] = data;
		//			//console.log(data['CarrierLotData'][0]);
		//			// draw lot info
		//			$.each(data['CarrierLotData'], function(index, carrierLotData) {
		//				var found = false;
		//				$.each(carrierLotData['LotList'], function(lotIndex, lotData) {
		//					var currentScheduleDate = lotData['ScheduleDate'];
							
		//					switch (dateQueryType) {
		//						case CART_SCHEDULE_PERIOD:
		//							if (currentScheduleDate >= dateStart && currentScheduleDate <= dateEnd) {
		//								found = true;
		//							}
		//							break;
		//						case CART_SCHEDULE_BEFORE:
		//							if (currentScheduleDate <= dateStart) {
		//								found = true;
		//							}
		//							break;
		//						case CART_SCHEDULE_AFTER:
		//							if (currentScheduleDate >= dateStart) {
		//								found = true;
		//							}
		//							break;
		//					}
							
		//					var currentWaitStatus = parseInt(lotData['WaitStatus']);
		//					//console.log(currentWaitStatus);
							
		//					if (currentWaitStatus > finalWaitStatus) {
		//						finalWaitStatus = currentWaitStatus;
		//					}
								
		//					switch (currentWaitStatus) {
		//						case CART_TIME_STATUS_NORMAL:
		//							textColot = CART_TIME_STATUS_COLOR_NORMAL;
		//							break;
		//						case CART_TIME_STATUS_HALF_OVERTIME:
		//							textColot = CART_TIME_STATUS_COLOR_HALF_OVERTIME;
		//							break;
		//						case CART_TIME_STATUS_OVERTIME:
		//							textColot = CART_TIME_STATUS_COLOR_OVERTIME;
		//							break;
		//					}
							
		//					// draw lot list
		//					if (lotString != '') {
		//						lotString = lotString + '<br>';
		//					}
		//					lotString = lotString + '<a id="' + lotData['Lot'] +
		//						'" style="color:' + textColot + ';" href="#" onclick="showLotData(\'' + cartNumber + '\', ' + index + ', ' + lotIndex + ')">' +
		//						lotData['Lot'] + '</a>';
		//				});
						
		//				//console.log(lotString);
		//				if (found) {
		//					// draw cart
		//					drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		//					updateCartStatus(cartNumber, finalWaitStatus);
		//					drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		//				}
		//			});
		//		},
		//		'json' // add this, the above 'data' variable from server has been parse to variable from JSON			
		//	).fail(function() {
		//		alert('讀取 MES 資料失敗');
		//		drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		//		drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		//	});
		//}

		//function addCartToParkingBlockWithWaitStatus(fieldId, parkingBlockId, cartNumber, waitStatus) {
		//	var lotString = '';
		//	var finalWaitStatus = CART_TIME_STATUS_NORMAL;
		//	$.post('http://172.16.3.180/EverestAPI04/Automation/GetCarrierDataAndLots',
		//		{Carrier: cartNumber},
		//		function (data) {
		//			carrierLotData[cartNumber] = data;
		//			//console.log(data['CarrierLotData'][0]['State']);
		//			// draw lot info
		//			$.each(data['CarrierLotData'], function(index, carrierLotData) {						
		//				$.each(carrierLotData['LotList'], function(lotIndex, lotData) {
		//					var currentWaitStatus = parseInt(lotData['WaitStatus']);
		//					//console.log(currentWaitStatus);
							
		//					if (currentWaitStatus > finalWaitStatus) {
		//						finalWaitStatus = currentWaitStatus;
		//					}
								
		//					switch (currentWaitStatus) {
		//						case CART_TIME_STATUS_NORMAL:
		//							textColot = CART_TIME_STATUS_COLOR_NORMAL;
		//							break;
		//						case CART_TIME_STATUS_HALF_OVERTIME:
		//							textColot = CART_TIME_STATUS_COLOR_HALF_OVERTIME;
		//							break;
		//						case CART_TIME_STATUS_OVERTIME:
		//							textColot = CART_TIME_STATUS_COLOR_OVERTIME;
		//							break;
		//					}
							
		//					// draw lot list
		//					if (lotString != '') {
		//						lotString = lotString + '<br>';
		//					}
		//					lotString = lotString + '<a id="' + lotData['Lot'] +
		//						'" style="color:' + textColot + ';" href="#" onclick="showLotData(\'' + cartNumber + '\', ' + index + ', ' + lotIndex + ')">' +
		//						lotData['Lot'] + '</a>';
		//				});
		//				//console.log(lotString);
		//				if (finalWaitStatus > CART_TIME_STATUS_NORMAL && (finalWaitStatus & waitStatus) == finalWaitStatus) {
		//					// draw cart
		//					drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		//					updateCartStatus(cartNumber, finalWaitStatus);
		//					drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		//				}
		//			});
		//		},
		//		'json' // add this, the above 'data' variable from server has been parse to variable from JSON			
		//	).fail(function() {
		//		alert('讀取 MES 資料失敗');
		//		drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		//		drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		//	});
		//}

		function addCartToParkingBlocks(fieldId, parkingBlockId, cartNumber, showStatus, selectedCart) {
		    var lotString = '';
		    var finalWaitStatus = CART_TIME_STATUS_NORMAL;
		    var textColot = CART_TIME_STATUS_COLOR_NORMAL;
		    drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		    drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		}

		//function addCartToParkingBlock(fieldId, parkingBlockId, cartNumber, showStatus, selectedCart) {
		//	var lotString = '';
		//	var finalWaitStatus = CART_TIME_STATUS_NORMAL;
		//	var textColot = CART_TIME_STATUS_COLOR_NORMAL;
		//	$.post('http://172.16.3.180/EverestAPI04/Automation/GetCarrierDataAndLots',
		//		{Carrier: cartNumber},
		//		function (data) {
		//			if (showStatus == '' || showStatus == data['CarrierLotData'][0]['State']) {
		//				carrierLotData[cartNumber] = data;
		//				//console.log(data['CarrierLotData'][0]);
		//				// draw lot info
		//				$.each(data['CarrierLotData'], function(index, carrierLotData) {
		//					$.each(carrierLotData['LotList'], function(lotIndex, lotData) {
		//						var currentWaitStatus = parseInt(lotData['WaitStatus']);
		//						if (currentWaitStatus > finalWaitStatus) {
		//							finalWaitStatus = currentWaitStatus;
		//						}
								
		//						switch (currentWaitStatus) {
		//							case CART_TIME_STATUS_NORMAL:
		//								textColot = CART_TIME_STATUS_COLOR_NORMAL;
		//								break;
		//							case CART_TIME_STATUS_HALF_OVERTIME:
		//								textColot = CART_TIME_STATUS_COLOR_HALF_OVERTIME;
		//								break;
		//							case CART_TIME_STATUS_OVERTIME:
		//								textColot = CART_TIME_STATUS_COLOR_OVERTIME;
		//								break;
		//						}
								
		//						if (lotString != '') {
		//							lotString = lotString + '<br>';
		//						}
		//						lotString = lotString + '<a id="' + lotData['Lot'] +
		//							'" style="color:' + textColot + ';" href="#" onclick="showLotData(\'' + cartNumber + '\', ' + index + ', ' + lotIndex + ')">' +
		//							lotData['Lot'] + '</a>';
		//						//console.log(lotString);
		//					});
							
		//					// draw cart
		//					if (selectedCart) {
		//						drawCart(fieldId, parkingBlockId, '#select_cart_g', cartNumber);
		//						// show blink of highlighted cart
		//						setCartBlink(cartNumber);
		//					} else {
		//						drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		//					}
		//					drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		//					if (finalWaitStatus > CART_TIME_STATUS_NORMAL) {
		//						updateCartStatus(cartNumber, finalWaitStatus);
		//					}
		//				});
		//			}
		//		},
		//		'json' // add this, the above 'data' variable from server has been parse to variable from JSON			
		//	).fail(function() {
		//		alert('讀取 MES 資料失敗');
		//		drawCart(fieldId, parkingBlockId, '#cart_g', cartNumber);
		//		drawPopover('#cart_' + cartNumber, cartNumber, lotString);
		//	});
		//}

		function AddPalletToParkingBlock(FieldID, PalletNumber) {
		    var lotString = '';
		    var finalWaitStatus = CART_TIME_STATUS_NORMAL;
		    var textColot = CART_TIME_STATUS_COLOR_NORMAL;

		    $.post('/Tracking/GetLotInfoByPalletNumber', { PalletNumber: PalletNumber },
				function (data) {
				    if (data.length > 0) {
				        carrierLotData[PalletNumber] = data;
				        $.each(data, function (lotIndex, lotData) {

				            var currentWaitStatus = parseInt(lotData['Status']);
				            if (currentWaitStatus > finalWaitStatus) {
				                finalWaitStatus = currentWaitStatus;
				            }

				            switch (currentWaitStatus) {
				                case CART_TIME_STATUS_NORMAL:
				                    textColot = CART_TIME_STATUS_COLOR_NORMAL;
				                    break;
				                case CART_TIME_STATUS_HALF_OVERTIME:
				                    textColot = CART_TIME_STATUS_COLOR_HALF_OVERTIME;
				                    break;
				                case CART_TIME_STATUS_OVERTIME:
				                    textColot = CART_TIME_STATUS_COLOR_OVERTIME;
				                    break;
				            }

				            if (lotString != '') {
				                lotString = lotString + '<br>';
				            }
				            lotString = lotString + '<a id="' + lotData['PLot'] +
                                '" style="color:' + textColot + ';" href="#" onclick="showLotData(\'' + PalletNumber + '\', ' + lotIndex + ')">' +
                                lotData['PLot'] + '</a>';
				        });

				        // draw cart
				        drawCart(FieldID, data[0]['ParkingBlockID'], '#cart_g', PalletNumber);
				        drawPopover('#cart_' + PalletNumber, PalletNumber, lotString);
				        if (finalWaitStatus > CART_TIME_STATUS_NORMAL) {
				            updateCartStatus(PalletNumber, finalWaitStatus);
				        }
				    }
				});
		}

		//function AddPalletToParkingBlock(fieldId, PalletInfo) {
		//    var lotString = '';
		//    var finalWaitStatus = CART_TIME_STATUS_NORMAL;
		//    var textColot = CART_TIME_STATUS_COLOR_NORMAL;
		//    var PalletNumber = PalletInfo['PalletNumber'];

		//    carrierLotData[PalletNumber] = PalletInfo;
		//    currentWaitStatus = 0;

		//    switch (currentWaitStatus) {
		//        case CART_TIME_STATUS_NORMAL:
		//            textColot = CART_TIME_STATUS_COLOR_NORMAL;
		//            break;
		//        case CART_TIME_STATUS_HALF_OVERTIME:
		//            textColot = CART_TIME_STATUS_COLOR_HALF_OVERTIME;
		//            break;
		//        case CART_TIME_STATUS_OVERTIME:
		//            textColot = CART_TIME_STATUS_COLOR_OVERTIME;
		//            break;
		//    }

		//    drawCart(fieldId, PalletInfo['ParkingBlockID'], '#cart_g', PalletNumber);

		//    drawPopover('#cart_' + PalletNumber, PalletNumber, lotString);

		//    if (finalWaitStatus > CART_TIME_STATUS_NORMAL) {
		//        updateCartStatus(PalletNumber, finalWaitStatus);
		//    }
		//}
		
		function drawParkingBlock(fieldId, id, x, y, width, height) {
			// create g tag
			var g = document.createElementNS('http://www.w3.org/2000/svg','g');
			g.setAttributeNS(null, 'id', id + '_g');
            $('#' + fieldId + '_svg #parkingBlock_g').append(g);

            // create a selected rectangle tag
            var rect = document.createElementNS('http://www.w3.org/2000/svg','rect');
			rect.setAttributeNS(null, 'id', 'parkingBlock_' + id);
            rect.setAttributeNS(null, 'x', x);
            rect.setAttributeNS(null, 'y', y);
            rect.setAttributeNS(null, 'width', width);
            rect.setAttributeNS(null, 'height', height);
//			rect.setAttributeNS(null, 'fill', '#4f81bd');
			rect.setAttributeNS(null, 'fill-opacity', 0);
			rect.setAttributeNS(null, 'fill', '#ff0000');
//			rect.setAttributeNS(null, 'fill-opacity', 0.5);
            $(g).append(rect);
		}
		
		function clearParkingBlock(fieldId) {
			$('#' + fieldId + '_svg #cart_g').empty();
			$('#' + fieldId + '_svg #select_cart_g').empty();
			if (fieldId in parkingBlockData) {
				var fieldParkingBlockData = parkingBlockData[fieldId];
				$.each(fieldParkingBlockData, function(index, value) {
					value['cartCount'] = 0;
				});
			}
		}
		
		function addParkingBlock(fieldId, parParkingBlockData) {
			parkingBlockData[fieldId] = parParkingBlockData;
			$.each(parkingBlockData[fieldId], function(index, value) {
				drawParkingBlock(fieldId, value['parkingBlockId'], value['x'], value['y'], value['width'], value['height']);
			});
		}
		
		function showUnselectedCartLayer(fieldId, state) {
			console.log('fieldId = ' + fieldId + ', state = ' + state);
			console.log($('#' + fieldId + '_svg .svg-pan-zoom_viewport #cart_g'));
			if (state) {			
				$('#' + fieldId + '_svg .svg-pan-zoom_viewport #cart_g').css('display', '');
			} else {
				$('#' + fieldId + '_svg .svg-pan-zoom_viewport #cart_g').css('display', 'none');
			}	
		}
	
        // create layer on map
        function createMapLayer(fieldId) {
			// create parking block layer
			var g = document.createElementNS('http://www.w3.org/2000/svg','g');
			g.setAttributeNS(null, 'id', 'parkingBlock_g');
			$('#' + fieldId + '_svg .svg-pan-zoom_viewport').append(g);
			
			// create unselected cart layer
			g = document.createElementNS('http://www.w3.org/2000/svg','g');
			g.setAttributeNS(null, 'id', 'cart_g');
			//g.setAttributeNS(null, 'class', 'hide');
			$('#' + fieldId + '_svg .svg-pan-zoom_viewport').append(g);

			// create select cart layer
			g = document.createElementNS('http://www.w3.org/2000/svg','g');
			g.setAttributeNS(null, 'id', 'select_cart_g');
			$('#' + fieldId + '_svg .svg-pan-zoom_viewport').append(g);
		}
        
        function createMapObject(fieldId) {
		    // ** map operations **
		    // create map layers
		    createMapLayer(fieldId);
        }