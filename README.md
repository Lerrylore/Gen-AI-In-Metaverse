# Gen-AI In Metaverse
This is the repository of _Gen-AI In Metaverse_ project, developed for the "Advanced User Interfaces" Course Project (prof. Franca Garzotto and tutor Alberto Patti) @ Politecnico di Milano, a.a. 2023/2024 evaluated with 30/30 _CumLaude_

## Overview
Gen-Ai in Metaverse is a VR application, integrated with Gen-Ai technology that addresses
social anxiety by providing a secure environment for users to enhance their social skills. Virtual
companions, crafted with empathy, facilitate realistic conversations to aid users in overcoming
social challenges. The platform utilizes Gen-Ai to create personalized, friendly avatars, enhancing
the interactions within a realistic virtual setting. Real-time communication is enabled through
Azure, while the interactions of each avatar are controlled by GPT APIs

## How To
To run the project on your machine, you need to have **Unity3D** installed on your machine, and to follow the instructions below:
1. create a `.env` file in the main folder (before `Assets/` folder) containing the following parameters:
    - `AzureKey` with the key for Azure Cognitive Speech Service;
    - `Region` with the selected region for Azure Cognitive service;
    - `OpenAI` with the token for the connection to the OpenAI API;
2. Open Unity and import the package SpeechSDK from Microsoft Cognitive Service, you can find it [here](https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/quickstart/csharp/unity/text-to-speech/README.md#download-the-speech-sdk-for-unity-and-the-sample-code).
3. To try the application you can either build the APK with Unity (the application has been tested with Meta Quest 2).
4. Or you can try the application directly from the editor plugging you VR device in.

## Video Demonstration
[![Video Demonstration](https://img.youtube.com/vi/9Re3KY5ppKw/0.jpg)](https://www.youtube.com/9Re3KY5ppKw)


## Credits
- [Lorenzo Corrado](mailto:lorenzo.corrado@mail.polimi.it)
- [Roberto Cialini](mailto:roberto.cialini@mail.polimi.it)
- [Filippo Pantaleone](mailto:filippo.pantaleone@mail.polimi.it)
- [Chukun Zhang](mailto:chukun.zhang@mail.polimi.it)

