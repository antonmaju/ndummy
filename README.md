NDummy
=====

NDummy is a .NET portable class library for generating object by spec.

[![Build status](https://ci.appveyor.com/api/projects/status/w5sqhvllq8nst7i6/branch/master)](https://ci.appveyor.com/project/AntonSetiawan/ndummy/branch/master)

##Installation##

To install NDummy, run the following command in the Package Manager Console

    PM> Install-Package NDummy

##Usage ##

<ul>
  <li>Create spec for classes you want to generate


    public class WarriorSpec : FactorySpec<Warrior>
    {
        public WarriorSpec()
        {
            ConfigureProperties(
               For(n =>n.Name).GenerateWith(ctx =>"Name-"+ctx.Index),
			   For(n =>n.Hp).GenerateWith(100) 	
              );
        }
    }


</li> 
<li>Register specs
	
    var config = new FactoriesConfig()
		.Register<Warrior, WarriorSpec>()
		.Register<Weapon, WeaponSpec>();

	var manager = new FactoryManager(config);

</li>	
<li>Get factory
    
	var factory = manager.GetFactory<Warrior>();
	var newInstance = factory.Create(); //create new Warrior instance 
</li>


Please visit [wiki](https://github.com/antonmaju/ndummy/wiki) for more information.
