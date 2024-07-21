using JetBrains.Annotations;
using Microsoft.CognitiveServices.Speech;
/*This class just provides a bunch of configurations for the APIs*/
public enum APIType
{
    OpenAI,
    Region,
    Azure
}
public struct Config
{
    

    private SpeechConfig speechConfig;

    public Config(string SubscriptionKey, string Region, string voiceOver)
    {
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        SetupSpeechConfig(voiceOver);
    }


    private void SetupSpeechConfig(string voice)
    {
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);
        speechConfig.SetProperty("style", "");
        speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "1000");
        speechConfig.SpeechSynthesisVoiceName = voice;
    }

    public SpeechConfig GetSpeechConfig()
    {
        return speechConfig;
    }
}