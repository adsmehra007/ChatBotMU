using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ChatBotApplication.Models;
using Microsoft.Bot.Builder;
using ChatBot.Controllers;
using ChatBot.Models;
using ChatBotApplication.Helper;
using ChatBotApplication;
using ChatBot.Helper;

namespace ChatBotApplication.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
           
            MessagesController obj = new MessagesController();
            var activity = await result as Activity;
            PatientController pC = new PatientController();
            BotManager bM = new BotManager();
            BOTResponse bR = new BOTResponse();
            var state = string.Empty;
            var input = string.Empty;
            var responseToken = string.Empty;
            string botResponse = string.Empty;
            input = activity.Text.ToString().ToLower();
            bool questionResponded = false;
            try
            {
                state = context.ConversationData.GetValue<string>("state");
                responseToken = context.ConversationData.GetValue<string>("ResponseToken");
            }
            catch (Exception ex)
            {

            }
            if (state == "firstname")
            {
                bR = pC.SearchPatient(input, 1, null, WebApiApplication.getPatData);
                if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "lastname");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "firstname");
                }
                await context.PostAsync($"" + bR.ResponseMessage);
                questionResponded = true;
            }
            if (state == "lastname")
            {
                bR = pC.SearchPatient(input, 2, null, WebApiApplication.getPatFirstName);
                if (WebApiApplication.getPatLastName.Count == 1)
                {
                    await context.PostAsync($"Thank you for Verifying your Details.");
                    await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    context.ConversationData.SetValue<string>("state", "options");
                }
                else if (bR.status == true)
                {
                    await context.PostAsync($"" + bR.ResponseMessage);
                    context.ConversationData.SetValue<string>("state", "dob");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    await context.PostAsync($"" + bR.ResponseMessage);
                    context.ConversationData.SetValue<string>("state", "");
                }
                questionResponded = true;
            }
            else if (state == "dob")
            {
                input = ConversionHelper.dateFormat(input);
                if (Validator.checkDateFormat(input))
                {
                    bR = pC.SearchPatient(input, 3, responseToken, WebApiApplication.getPatLastName);
                    if (bR.status == true)
                    {

                        if (WebApiApplication.getPatDOB.Count == 1)
                        {
                            await context.PostAsync($"Thank you for Verifying your Details.");
                            await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                            context.ConversationData.SetValue<string>("state", "options");
                        }
                        else
                        {
                            bool hasSSN = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.SSN));
                            bool hasHomePhone = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.PatHomePhone));
                            bool hasWorkPhone = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.PatWorkPhone));
                            if (hasHomePhone || hasWorkPhone)
                            {
                                context.ConversationData.SetValue<string>("state", "phone");
                                context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                                await context.PostAsync($"Would you like to share your contact with me ?");
                            }
                            else if (hasSSN)
                            {
                                context.ConversationData.SetValue<string>("state", "ssn");
                                context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                                await context.PostAsync($"" + bR.ResponseMessage);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "dob");
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "dob");
                    await context.PostAsync($"Please enter valid Date format, (MM/DD/YYYY)");
                }
                questionResponded = true;
            }
            else if (state == "phone")
            {
                if (Validator.isPhoneNumberValid(input))
                {
                    bR = pC.SearchPatient(input, 6, responseToken, WebApiApplication.getPatDOB);
                    if (WebApiApplication.getPatPhone.Count == 1)
                    {
                        await context.PostAsync($"Thank you for Verifying your Details.");
                        await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                        context.ConversationData.SetValue<string>("state", "options");
                    }
                    else if (bR.status == true)
                    {
                        context.ConversationData.SetValue<string>("state", "ssn");
                        context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "");
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "phone");
                    await context.PostAsync($"Please enter valid Phone format, (xxxxxxxxxx)");
                }
                questionResponded = true;
            }

            else if (state == "zip")
            {
                bR = pC.SearchPatient(input, 5, responseToken, WebApiApplication.getPatZIP);
                if (WebApiApplication.getPatZIP.Count == 1)
                {
                    await context.PostAsync($"Thank you for Verifying your Details.");
                    await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    context.ConversationData.SetValue<string>("state", "options");
                }
                else if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    await context.PostAsync($"" + bR.ResponseMessage);
                    await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    context.ConversationData.SetValue<string>("state", "options");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    await context.PostAsync($"" + bR.ResponseMessage);
                }
                questionResponded = true;
            }
            else if (state == "ssn")
            {
                bR = pC.SearchPatient(input, 4, responseToken, WebApiApplication.getPatDOB);
                if (WebApiApplication.getPatSSN.Count == 1)
                {
                    await context.PostAsync($"Thank you for Verifying your Details.");
                    await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    context.ConversationData.SetValue<string>("state", "options");
                }
                else if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                    await context.PostAsync($"" + bR.ResponseMessage);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "ssn");
                    await context.PostAsync($"" + bR.ResponseMessage);
                }
                questionResponded = true;
            }
           
            else if (state == "options")
            {
                if (input.ToLower() == "a")
                {
                    await context.PostAsync($"You chose to book an Appointment.");


                }
                else if (input.ToLower() == "b")
                {
                    await context.PostAsync($"You chose to search patient visit history.");


                }
                else if (input.ToLower() == "c")
                {
                    await context.PostAsync($"You chose to pay bill.");


                }
                else if (input.ToLower() == "d")
                {
                    await context.PostAsync($"You chose to refill a request.");

                }
                else
                {
                    //await context.PostAsync($"Sorry, Cannot process this input at the moment.");
                    try
                    {
                        var commonQuestions = bM.getCommonQuestions();
                        foreach (var cQuestion in commonQuestions)
                        {
                            try
                            {
                                bool isMatch = Validator.sentenceComparison(cQuestion.Question, input);
                                if (isMatch == true)
                                {
                                    botResponse = cQuestion.Answer;
                                    await context.PostAsync(botResponse);
                                    context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                    questionResponded = true;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                questionResponded = true;
            }
            else
            {
                try
                {
                    var commonQuestions = bM.getCommonQuestions();
                    foreach (var cQuestion in commonQuestions)
                    {
                        try
                        {
                            bool isMatch = Validator.sentenceComparison(cQuestion.Question, input);
                            if (isMatch == true)
                            {
                                botResponse = cQuestion.Answer;
                                await context.PostAsync(botResponse);
                                context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                questionResponded = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }
                catch (Exception ex)
                {

                }
            }
            if (questionResponded == false)
            { await context.PostAsync($"Bot is running");
               // await context.PostAsync($"Sorry, Cannot process this input at the moment.");

            }

            //if (input.Contains("hey") || input.Contains("hello"))
            //{
            //    await context.PostAsync($"What is your name ?");
            //    context.ConversationData.SetValue<string>("State","name");
            //}


            //else if (ShortName == null)
            //{
            //    await context.PostAsync($"The ICDcodes is Invalid..Please Enter Valid ICDcode ");
            //}
            //else
            //{
            //    await context.PostAsync($"The ICDcodes is  {activity.Text} and its shortname is  {ShortName} ");
            //    await context.PostAsync($"Do you want to search something else ...press yes or no!!");

            //}
            // Return our reply to the user


            context.Wait(MessageReceivedAsync);
        }

    }
}
