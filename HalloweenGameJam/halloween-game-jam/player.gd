extends CharacterBody2D


const maxSpeed = 400
const acceleration = 1500
const friction = 600

var input = Vector2.ZERO

func _physics_process(delta):
	playerMovement(delta)

func getInput():
	input.x = int(Input.is_action_pressed("ui_right")) - int(Input.is_action_pressed("ui_left"))
	input.y = int(Input.is_action_pressed("ui_down")) - int(Input.is_action_pressed("ui_up"))
	return input.normalized()
	
func playerMovement(delta):
	input = getInput()
	if input == Vector2.ZERO:
		if velocity.length() > (friction * delta):
			velocity -= velocity.normalized() * (friction * delta)
		else:
			velocity = Vector2.ZERO
	else:
		velocity += (input * acceleration * delta)
		velocity = velocity.limit_length(maxSpeed)
		
	move_and_slide()
