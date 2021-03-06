function [nodes, I, O, F, T] = getNodeCoordArray(str) 
    strings = strsplit(str, ';');
    cellSize = size(strings)-1;
    nodes = zeros(cellSize(2), 4);
    I = [];
    O = [];
    F = [];
    T = [];
    for i=1:(cellSize(2))
        tempString = strings{i};
        nodes(i,1) = str2double(getIndex(tempString));
        typeString = tempString(length(tempString));
        if(strcmp(typeString, 'I'))
            I = [I i];
        elseif(strcmp(typeString, 'O'))
            O = [O i];
        elseif(strcmp(typeString, 'T'))
            T = [T i];
        elseif(strcmp(typeString, 'F'))
            F = [F i];
        end
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
    coordString = str(start+1:fin-1);
    coordsCell = str2double(strsplit(coordString,','));
end

function indexString = getIndex(str)
    endIndex = 0
    for i=1:length(str)
        if strcmp(str(i),'(') == 1
            endIndex = i - 1
            break
        end
    end
    indexString = str(1:endIndex);
end