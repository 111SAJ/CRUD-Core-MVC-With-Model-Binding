using Candidate.Data;
using Candidate.Models;
using Candidate.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Candidate.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            // Fetch the list of employees including their associated skills
            var employees = _context.Employee
                .Include(e => e.State)
                .Include(e => e.City)
                .Include(e => e.EmployeeSkill)
                .Select(e => new EmployeeViewModel
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Mobile = e.Mobile,
                    Address = e.Address,
                    Gender = e.Gender,
                    Profile = e.Profile,
                    StateName = e.State.StateName, // Mapping StateName to render on Index page only
                    CityName = e.City.CityName,    // Mapping CityName to render on Index page only
                    SelectedSkillIds = e.EmployeeSkill.Select(es => es.SkillId).ToList()
                })
                .ToList();

            return View(employees);
        }

        // Employee/Create
        [HttpGet]
        public IActionResult Create()
        {

            var employeeViewModel = new EmployeeViewModel
            {
                States = new SelectList(_context.State.ToList(), "StateId", "StateName"),
                Cities = new SelectList(Enumerable.Empty<City>(), "CityId", "CityName"),
                Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName")
            };
            return View(employeeViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeViewModel, IFormFile profileFile)
        {
            // If a state is selected, populate the cities for that state
            if (employeeViewModel.StateId.HasValue)
            {
                employeeViewModel.Cities = new SelectList(_context.City
                    .Where(c => c.StateId == employeeViewModel.StateId).ToList(), "CityId", "CityName");
            }
            else
            {
                employeeViewModel.Cities = new SelectList(Enumerable.Empty<City>(), "CityId", "CityName");
            }

            // Re-populate the states dropdown for the view
            employeeViewModel.States = new SelectList(_context.State.ToList(), "StateId", "StateName");

            // Re-populate the skills for the view
            employeeViewModel.Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName");

            if (ModelState.IsValid)
            {

                // Validate if the CityId matches the selected StateId
                if (employeeViewModel.StateId.HasValue && employeeViewModel.CityId.HasValue)
                {
                    var selectedCity = _context.City.Find(employeeViewModel.CityId);
                    if (selectedCity == null || selectedCity.StateId != employeeViewModel.StateId.Value)
                    {
                        ModelState.AddModelError("CityId", "The selected city does not match the selected state.");
                        // Re-populate the dropdown list if in case ModelState is Invalid
                        employeeViewModel.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
                        employeeViewModel.Cities = new SelectList(_context.City.Where(c => c.StateId == employeeViewModel.StateId).ToList(), "CityId", "CityName");
                        employeeViewModel.Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName");
                        return View(employeeViewModel);
                    }
                }

                //check if employee already exist
                var existEmployee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == employeeViewModel.Email);
                if (existEmployee != null)
                {
                    ModelState.AddModelError("Email", "Email already exist");
                    employeeViewModel.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
                    employeeViewModel.Cities = new SelectList(_context.City.Where(c => c.StateId == employeeViewModel.StateId).ToList(), "CityId", "CityName");
                    employeeViewModel.Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName");
                    return View(employeeViewModel);
                }

                // Handle file upload
                if (profileFile != null)
                {
                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadDir);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileFile.CopyToAsync(fileStream);
                    }

                    employeeViewModel.Profile = fileName; // Store the file name or path
                }

                var employee = new Employee
                {
                    Name = employeeViewModel.Name,
                    Email = employeeViewModel.Email,
                    Mobile = employeeViewModel.Mobile,
                    StateId = employeeViewModel.StateId.Value,
                    CityId = employeeViewModel.CityId.Value,
                    Address = employeeViewModel.Address,
                    Gender = employeeViewModel.Gender,
                    Profile = employeeViewModel.Profile
                };

                foreach (var skillId in employeeViewModel.SelectedSkillIds)
                {
                    employee.EmployeeSkill.Add(new EmployeeSkill { SkillId = skillId });
                }

                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // If model is invalid, reload the form with existing states and cities
            employeeViewModel.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
            employeeViewModel.Cities = new SelectList(_context.City.Where(c => c.StateId == employeeViewModel.StateId).ToList(), "CityId", "CityName");
            employeeViewModel.Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName");
            return View(employeeViewModel);
        }

        //Update
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _context.Employee
                .Include(e => e.EmployeeSkill)
                .FirstOrDefault(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel = new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Mobile = employee.Mobile,
                Address = employee.Address,
                Gender = employee.Gender,
                StateId = employee.StateId,
                CityId = employee.CityId,
                Profile = employee.Profile,
                States = new SelectList(_context.State.ToList(), "StateId", "StateName"),
                Cities = new SelectList(_context.City.Where(c => c.StateId == employee.StateId).ToList(), "CityId", "CityName"),
                Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName"),
                SelectedSkillIds = employee.EmployeeSkill.Select(es => es.SkillId).ToList()
            };

            return View(employeeViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel employeeViewModel, IFormFile profileFile)
        {
            if (ModelState.IsValid)
            {
                var existingEmployee = _context.Employee
                    .Include(e => e.EmployeeSkill)
                    .FirstOrDefault(e => e.EmployeeId == id);

                // Validate if the CityId matches the selected StateId
                if (employeeViewModel.StateId.HasValue && employeeViewModel.CityId.HasValue)
                {
                    var selectedCity = _context.City.Find(employeeViewModel.CityId);
                    if (selectedCity == null || selectedCity.StateId != employeeViewModel.StateId.Value)
                    {
                        ModelState.AddModelError("CityId", "The selected city does not match the selected state.");
                        // Re-populate the dropdown list if in case ModelState is Invalid
                        employeeViewModel.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
                        employeeViewModel.Cities = new SelectList(_context.City.Where(c => c.StateId == employeeViewModel.StateId).ToList(), "CityId", "CityName");
                        employeeViewModel.Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName");
                        return View(employeeViewModel);
                    }
                }

                // Update existing fields
                existingEmployee.Name = employeeViewModel.Name;
                existingEmployee.Email = employeeViewModel.Email;
                existingEmployee.Mobile = employeeViewModel.Mobile;
                existingEmployee.Address = employeeViewModel.Address;
                existingEmployee.Gender = employeeViewModel.Gender;
                existingEmployee.StateId = employeeViewModel.StateId.Value;
                existingEmployee.CityId = employeeViewModel.CityId.Value;

                // Handle file upload if a new file is provided
                if (profileFile != null)
                {
                    if (!string.IsNullOrEmpty(existingEmployee.Profile))
                    {
                        var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", existingEmployee.Profile);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadDir);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileFile.CopyToAsync(fileStream);
                    }

                    existingEmployee.Profile = fileName;
                }

                // Update skills
                existingEmployee.EmployeeSkill.Clear();
                foreach (var skillId in employeeViewModel.SelectedSkillIds)
                {
                    existingEmployee.EmployeeSkill.Add(new EmployeeSkill { SkillId = skillId });
                }

                _context.Update(existingEmployee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Repopulate dropdown lists and return the view if the model is invalid
            employeeViewModel.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
            employeeViewModel.Cities = new SelectList(_context.City.Where(c => c.StateId == employeeViewModel.StateId).ToList(), "CityId", "CityName");
            employeeViewModel.Skills = new SelectList(_context.Skill.ToList(), "SkillId", "SkillName");

            return View(employeeViewModel);
        }

        //Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var existEmployee = _context.Employee
                .Include(e => e.State)
                .Include(e => e.City)
                .Include(e => e.EmployeeSkill)
                .FirstOrDefault(e => e.EmployeeId == id);

            if (existEmployee == null)
            {
                return NotFound();
            }

            // Map the Employee entity to EmployeeViewModel
            var employeeViewModel = new EmployeeViewModel
            {
                EmployeeId = existEmployee.EmployeeId,
                Name = existEmployee.Name,
                Email = existEmployee.Email,
                Mobile = existEmployee.Mobile,
                Address = existEmployee.Address,
                Gender = existEmployee.Gender,
                Profile = existEmployee.Profile,
                StateId = existEmployee.StateId,
                StateName = existEmployee.State?.StateName,
                CityId = existEmployee.CityId,
                CityName = existEmployee.City?.CityName,
                SelectedSkillIds = existEmployee.EmployeeSkill.Select(es => es.SkillId).ToList()
            };

            return View(employeeViewModel);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var existEmployee = _context.Employee
                                .Include(e => e.EmployeeSkill) // Include related EmployeeSkill records
                                .FirstOrDefault(e => e.EmployeeId == id);

            if (existEmployee != null)
            {
                // Delete related EmployeeSkill records
                _context.EmployeeSkill.RemoveRange(existEmployee.EmployeeSkill);

                // Delete the profile image if it exists
                if (!string.IsNullOrEmpty(existEmployee.Profile))
                {
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", existEmployee.Profile);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Employee.Remove(existEmployee);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
