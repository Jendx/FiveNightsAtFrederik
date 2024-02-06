extends Node3D

var time = 0.0
var current_light = null
var all_lights
var light_selection_timer
var tween 

# Called when the node enters the scene tree for the first time.
func _ready():
	all_lights = $Lights.get_children()
	light_selection_timer = $LightSelectionTimer
	

func _on_light_timer_timeout():
	#Stop the selection timer
	light_selection_timer.stop()
	
	print("-------------------------------------")
	print("Timer stopped")
	
	#Get random time and number of flashes
	var time_between_selections = randf_range(1, 5)
	var number_of_flashes = randi_range(1, 5)
	
	current_light = all_lights[randi() % all_lights.size()]

	
	#check if there is at least one light
	if current_light != null :
		
		print(number_of_flashes)
		
		#flash the number of times
		for flashes in number_of_flashes:
			
			var time_between_flashes = randf_range(0.1, 0.5)
			
			current_light.light_energy = 1.0
			await get_tree().create_timer(time_between_flashes).timeout
			
			#turn the light off
			current_light.light_energy = 0.0
			print(time_between_flashes)
			
			#Wait a sec to flash the light
			await get_tree().create_timer(time_between_flashes).timeout
			
			#turn the light on
			current_light.light_energy = 1.0
			
			print("Flashed")
			
	#set new time for the selection timer
	light_selection_timer.set_wait_time(time_between_selections)
		
	#Start the selection timer to chose the new light to flash
	light_selection_timer.start()
	print("Timer waiting to chose a light")
	print("-------------------------------------")
		#urrent_light.light_energy = 1.0 - time / flash_duration  # Gradually turn the light back on

