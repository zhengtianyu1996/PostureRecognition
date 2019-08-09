clear

% path_data='./data/';
% path_data_train='./data/train/';
% path_data_val='./data/val/';
% 
% mkdir(path_data)
% mkdir(path_data_train)
% mkdir(path_data_val)

path_data_zp='./data_zp/';
fileExtension='.txt';
txt_file_list = dir(fullfile(path_data_zp, "*", "*" + fileExtension));

numObservations = numel(txt_file_list);
files = strings(numObservations,1);
labels = cell(numObservations,1);

for i = 1:numObservations
    name = txt_file_list(i).name;
    folder = txt_file_list(i).folder;
    
    [~,labels{i}] = fileparts(folder);
    files(i) = fullfile(folder,name);
end

labels = categorical(labels);


sequences = cell(numObservations,1);
for i = 1:numObservations
%         fprintf("Reading file %d of %d...\n", i, numObservations)
        
        fileID = fopen(files(i));
        dat=textscan(fileID,'%f');
        sequences{i,1}=reshape(dat{1},75,60);
%         sequences{i,1}=dat{1};
        fclose(fileID);
end


idx = randperm(numObservations);
N = floor(0.8 * numObservations);

idxTrain = idx(1:N);
sequencesTrain = sequences(idxTrain);
labelsTrain = labels(idxTrain);

idxValidation = idx(N+1:end);
sequencesValidation = sequences(idxValidation);
labelsValidation = labels(idxValidation);



numFeatures = size(sequencesTrain{1},1);
numClasses = numel(categories(labelsTrain));
numHiddenUnits=500;

layers = [
    sequenceInputLayer(numFeatures,'Name','sequence')
    bilstmLayer(numHiddenUnits,'OutputMode','last','Name','bilstm')
    dropoutLayer(0.3,'Name','drop')
    fullyConnectedLayer(4096,'Name','fc4')
    dropoutLayer(0.1,'Name','drop1')
    fullyConnectedLayer(512,'Name','fc3')
%     dropoutLayer(0.1,'Name','drop2')
%     fullyConnectedLayer(64,'Name','fc2')
%     dropoutLayer(0.8,'Name','drop3')
    fullyConnectedLayer(numClasses,'Name','fc1')
    softmaxLayer('Name','softmax')
    classificationLayer('Name','classification')];

miniBatchSize = 32;
maxEpochs=25;
% numObservations = numel(sequencesTrain);
numIterationsPerEpoch = floor(numObservations / miniBatchSize);

options = trainingOptions('rmsprop', ...
    'MiniBatchSize',miniBatchSize, ...
    'ExecutionEnvironment', 'gpu', ...
    'MaxEpochs',maxEpochs, ...
    'InitialLearnRate',1e-4, ...
    'LearnRateSchedule','piecewise', ...
    'LearnRateDropFactor',0.1, ...
    'LearnRateDropPeriod',15, ...
    'GradientThreshold',1, ...
    'Shuffle','every-epoch', ...
    'ValidationData',{sequencesValidation,labelsValidation}, ...
    'ValidationFrequency',numIterationsPerEpoch, ...
    'Plots','training-progress', ...
    'Verbose',false);

[netLSTM,info] = trainNetwork(sequencesTrain,labelsTrain,layers,options);

index=160;
X = sequencesValidation{index};
numTimeSteps = size(X,2);
lab=zeros(1, numTimeSteps);
la=["SitDown","Sitting","StandUp","Standing","TurnBack","Walking"];
% for i = 1:numTimeSteps
%     v = X(:,i);
%     [net,label,score] = classifyAndUpdateState(netLSTM,v);
%     lab(i) = label;
% end

[net,label,score] = classifyAndUpdateState(netLSTM,X(:,20:50));
lab(i) = label;
    
figure
stairs(lab, '-o')
xlim([1 numTimeSteps])
xlabel("Time Step")
ylabel("Predicted Class")
title("Classification Over Time Steps")


trueLabel = string(labelsValidation(index));
ind = find(la==trueLabel);

hold on
line([1 numTimeSteps],[ind ind], ...
    'Color','red', ...
    'LineStyle','--')
legend(["Prediction" "True Label"])

YPred = classify(netLSTM,sequencesValidation,'MiniBatchSize',miniBatchSize);
YValidation = labelsValidation;
accuracy = mean(YPred == YValidation)