using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

class Program
{
    // Set the speech service subscription key and region
    static string speechKey = "De6ccffd779146abb0524c7c37c0c717";
    static string speechRegion = "westus";
    async static Task Main(string[] args)
    {
        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
        speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
        speechTranslationConfig.AddTargetLanguage("fr");

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

        Console.WriteLine("Speak now...");
        var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
        OutputSpeechRecognitionResult(translationRecognitionResult);
    }
    static void OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
    {
        switch (translationRecognitionResult.Reason)
        {
            case ResultReason.TranslatedSpeech:
                Console.WriteLine($"Recognized spoken text: Text={translationRecognitionResult.Text}");
                foreach (var element in translationRecognitionResult.Translations)
                {
                    Console.WriteLine($"Translating into '{element.Key}': {element.Value}");
                }
                break;
            case ResultReason.NoMatch:
                Console.WriteLine($"No match: Speech could not be recognized.");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(translationRecognitionResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
    }
}