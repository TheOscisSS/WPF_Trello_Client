using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public class Board : BindableBase
    {
        public Board(
            string id,
            string title,
            string description,
            string background,
            DateTime createdAt,
            DateTime updatedAt)
        {
            ID = id;
            Title = title;
            Description = description;
            Background = background;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public Board(
             User owner,
             ObservableCollection<User> members,
             ObservableCollection<BoardList> lists)
        {
            Members = members;
            Lists = lists;
        }

        public Board(
           string id,
           string title,
           string description,
           string background,
           User owner,
           ObservableCollection<User> members,
           ObservableCollection<BoardActivity> activities,
           DateTime createdAt,
           DateTime updatedAt,
           ObservableCollection<BoardList> lists)
        {
            ID = id;
            Title = title;
            Description = description;
            Background = background;
            Owner = owner;
            Members = members;
            Activities = activities;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Lists = lists;
        }
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Background { get; private set; }
        public User Owner { get; private set; }
        public ObservableCollection<User> Members { get; private set; }
        public ObservableCollection<BoardActivity> Activities { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public ObservableCollection<BoardList> Lists { get; private set; }

        public void UpdateEachActivityIcon(string icon, string userID)
        {
            foreach(var activity in Activities)
            {
                if(activity.Sender.ID == userID)
                {
                    activity.SetIcon(icon);
                }
            }
        }

        public void AddNewList(BoardList newList)
        {
            Lists.Add(newList);
        }
        public void AddNewActivity(BoardActivity newActivity)
        {
            Activities.Insert(0, newActivity);
        }
        public void AddNewMember(User newMember)
        {
            Members.Add(newMember);
        }
    }
}
