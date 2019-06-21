using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using DiscordAPI.SharedModels;

namespace Quarrel.Managers
{
    public class ContactManager
    {
        /// <summary>
        /// Setup manager on creation
        /// </summary>
        public ContactManager()
        {
            Setup();
        }

        /// <summary>
        /// Setup ContactManager
        /// </summary>
        public async void Setup()
        {
            await GetContactList();
            await GetContactAnnotationList();
        }

        /// <summary>
        /// Contact store
        /// </summary>
         ContactStore store;

        /// <summary>
        /// Contact annotations store
        /// </summary>
         ContactAnnotationStore annotationStore;

        /// <summary>
        /// Contact list 
        /// </summary>
         ContactList contactList;

        /// <summary>
        /// Contact annotations list
        /// </summary>
         ContactAnnotationList annotationList;

        /// <summary>
        /// Load contact list
        /// </summary>
        /// <returns>Contact list</returns>
        private async Task<ContactList> GetContactList()
        {
            // If contact list is already determined, just return it
            if (contactList == null)
            {
                // Initialize contact store
                store = await Windows.ApplicationModel.Contacts.ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

                if (store == null)
                {
                    // Unavailable, call it quits
                    return null;
                }

                // Get contact list
                IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();
                if (contactLists.Count == 0)
                {
                    contactList = await store.CreateContactListAsync("Discord");
                }
                else
                {
                    contactList = contactLists[0];
                }
            }
            
            // Return contact list
            return contactList;
        }

        /// <summary>
        /// Load ContactAnnotation List
        /// </summary>
        /// <returns></returns>
        private async Task<ContactAnnotationList> GetContactAnnotationList()
        {
            // If annotation list is already determined, return it
            if (annotationList == null)
            {
                // Initialize annotations contact store
                annotationStore = await Windows.ApplicationModel.Contacts.ContactManager.RequestAnnotationStoreAsync(ContactAnnotationStoreAccessType.AppAnnotationsReadWrite);

                if (annotationStore == null)
                {
                    // Unavailable, call it quits
                    return null;
                }

                // Get annotations list
                IReadOnlyList<ContactAnnotationList> annotationLists = await annotationStore.FindAnnotationListsAsync();
                if (annotationLists.Count == 0)
                {
                    annotationList = await annotationStore.CreateAnnotationListAsync();
                }
                else
                {
                    annotationList = annotationLists[0];
                }
            }

            return annotationList;
        }

        /// <summary>
        /// Get contact by Id
        /// </summary>
        /// <param name="id">ContactId</param>
        /// <returns>Contact of Id</returns>
        public async Task<Contact> GetContact(string id)
        {
            return await (await GetContactList()).GetContactFromRemoteIdAsync(id);
        }

        /// <summary>
        /// Get DiscordId from ContactId
        /// </summary>
        /// <param name="id">ContactId</param>
        /// <returns>DiscordId</returns>
        public async Task<string> ContactIdToRemoteId(string id)
        {
            if (store == null)
            {
                //Unavailable, call it quits
                return string.Empty;
            }

            // Get contact annotations
            var fullContact = await store.GetContactAsync(id);
            var contactAnnotations = await (await Windows.ApplicationModel.Contacts.ContactManager.RequestAnnotationStoreAsync(ContactAnnotationStoreAccessType.AppAnnotationsReadWrite)).FindAnnotationsForContactAsync(fullContact);

            // If contact contains annotations
            if (contactAnnotations.Count >= 0)
            {
                // Return RemoteId
                return contactAnnotations[0].RemoteId;
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if contact exists
        /// </summary>
        /// <param name="user">User to check</param>
        /// <returns>True if the user does not exist</returns>
        private async Task<bool> CheckContact(User user)
        {
            // Intialize store if not done
            if (store == null)
            {
                store = await Windows.ApplicationModel.Contacts.ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            }

            if (store == null)
            {
                // Unavailable, call it quits
                return true;
            }

            ContactList contactList;

            // Get contact lists for Discord
            IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();
            if (contactLists.Count == 0)
            {
                contactList = await store.CreateContactListAsync("Discord");
            }
            else
            {
                contactList = contactLists[0];
            }

            // Get user
            var returnval = await contactList.GetContactFromRemoteIdAsync(user.Id);

            // If user is null, return true
            return returnval != null;
        }
        
        /// <summary>
        /// Add contact
        /// </summary>
        /// <param name="user">Discord User</param>
        public async Task AddContact(User user)
        {
            if (!await CheckContact(user))
            {
                // Create contact
                Contact contact = new Contact();
                contact.Name = user.Username + "#" + user.Discriminator;
                contact.RemoteId = user.Id;
                contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromUri(Common.AvatarUri(user.Avatar, user.Id));

                // Save the contacts
                ContactList contactList = await GetContactList();
                if (null == contactList)
                {
                    return;
                }
                try
                {
                    await contactList.SaveContactAsync(contact);
                }
                catch
                {
                    // :shrug:
                }

                if (annotationList == null)
                {
                    return;
                }

                // Create annotations for contact
                ContactAnnotation annotation = new ContactAnnotation();
                annotation.RemoteId = user.Id;
                annotation.ContactId = contact.Id;
                annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile | ContactAnnotationOperations.Message | ContactAnnotationOperations.Share;
                annotation.ProviderProperties.Add("ContactPanelAppID", Windows.ApplicationModel.Package.Current.Id.FamilyName + "!App");

                // Save annotations on contact
                if(!await annotationList.TrySaveAnnotationAsync(annotation))
                {
                    Debug.WriteLine("Failed to save contact " + user.Username);
                }
            }
        }

        /// <summary>
        /// Add contact by Discord friend object
        /// </summary>
        /// <param name="user">Discord User</param>
        public async void AddContact(Friend user)
        {
            // Create contact
            Contact contact = new Contact();
            contact.FirstName = user.user.Username;
            contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromUri(Common.AvatarUri(user.user.Avatar, user.Id));
            
            // Save contact
            if (contactList == null)
            {
                return;
            }
            await contactList.SaveContactAsync(contact);

            ContactAnnotationList annotationList = await GetContactAnnotationList();
            if (annotationList == null)
            {
                return;
            }

            // Creeate annotations of contact
            ContactAnnotation annotation = new ContactAnnotation();
            annotation.ContactId = contact.Id;
            annotation.RemoteId = user.Id;
            annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile | ContactAnnotationOperations.Message | ContactAnnotationOperations.Share;

            // Save annotations on contact
            if (!await annotationList.TrySaveAnnotationAsync(annotation))
            {
                return;
            }
        }
    }
}
