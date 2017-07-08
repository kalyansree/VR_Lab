function [vertString, edgeString] = ReadFromUnity(tcpipServer)
    buildString = '';
    vertString = '';
    edgeString = '';
    buildString = fread(tcpipServer,1,'char');
    while(1)
        if get(tcpipServer,'BytesAvailable') > 0
            rawData = fread(tcpipServer, 1, 'char');
            if rawData == ' '
                buildString = strcat({char(buildString)}, {' '});
            elseif rawData == '|' % A '|' character indicates that we should break and start reading a new string %
                break;
            else
                buildString = strcat({char(buildString)}, char(rawData));
            end
        else
            break;
        end
    end
    vertString = buildString{1};
        
    %Now we start reading the 2nd string%
    buildString = '';
    buildString = fread(tcpipServer,1,'char');
    while(1)
        if get(tcpipServer,'BytesAvailable') > 0
            rawData = fread(tcpipServer, 1, 'char');
            if rawData == ' '
                buildString = strcat({char(buildString)}, {' '});
            elseif rawData == '|' 
                break;
            else
                buildString = strcat({char(buildString)}, char(rawData));
            end
        else
            break;
        end
    end
    edgeString = buildString{1};
end