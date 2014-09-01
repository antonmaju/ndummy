NDummy
=====

NDummy is a .NET portable class library for generating object by spec.

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


For detailed documentation please visit NDummy project pages.
