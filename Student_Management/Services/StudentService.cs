using MongoDB.Driver;
using Student_Management.Models;

namespace Student_Management.Services
{
    public class StudentService : IStudentService
    {
        private readonly IMongoCollection<Student> _students;
        private IMongoClient? mongoClient;
        private IStudentStoreDatabaseSettings? studentStoreDatabaseSettings;

        public StudentService(IStudentStoreDatabaseSettings settings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _students = database.GetCollection<Student>(settings.StudentCoursesCollectionName);
        }

        public StudentService(IMongoClient mongoClient, IStudentStoreDatabaseSettings studentStoreDatabaseSettings)
        {
            this.mongoClient = mongoClient;
            this.studentStoreDatabaseSettings = studentStoreDatabaseSettings;
            var database = mongoClient.GetDatabase(studentStoreDatabaseSettings.DatabaseName);
            _students = database.GetCollection<Student>(studentStoreDatabaseSettings.StudentCoursesCollectionName);
        }

        public Student Create(Student student)
        {
            try
            {
                _students.InsertOne(student);
            }
            catch (MongoDB.Bson.BsonSerializationException ex)
            {
                var message = $"Error creating student with Id '{student.Id}': {ex.Message}";
                throw new ArgumentException(message, ex);
            }
            return student;
        }

        public List<Student> Get()
        {
            return _students.Find(student => true).ToList();
        }

        public Student Get(string id)
        {
            return _students.Find(student => student.Id == id).FirstOrDefault();
        }

        public void Remove(string id)
        {
            _students.DeleteOne(student => student.Id == id);
        }

        public void Update(string id, Student student)
        {
            _students.ReplaceOne(student => student.Id == id, student);
        }
    }
}
