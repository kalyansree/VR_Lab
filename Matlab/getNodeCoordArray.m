function [nodes] = getNodeCoordArray(str) 
    strings = strsplit(str, ';');
    cellSize = size(strings)-1;
    nodes = zeros(cellSize(2), 4);
    for i=1:(cellSize(2))
        tempString = strings{i};
        nodes(i,1) = str2double(tempString(1));
        nodes(i,2:4) = getCoordsFromString(tempString);
    end
end

function [coordsCell] = getCoordsFromString(str)
    start = 0;
    fin = 0;
    for i=1:length(str)
        if strcmp(str(i),'(') == 1
            start = i;
        elseif strcmp(str(i),')') == 1
            fin = i;
        end
    end
    coordString = str([start+1:fin-1]);
    coordsCell = str2double(strsplit(coordString,','));
end