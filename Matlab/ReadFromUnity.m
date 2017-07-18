function [vertString, edgeString, forceString] = ReadFromUnity(tcpipServer)

    vertString = readNextString(tcpipServer);
    edgeString = readNextString(tcpipServer);
    forceString = readNextString(tcpipServer);
end

function finalString = readNextString(tcpipServer)
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
    finalString = buildString{1};
end